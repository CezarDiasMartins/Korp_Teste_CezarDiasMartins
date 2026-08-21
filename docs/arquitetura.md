# Detalhamento Técnico da Solução

## 1. Visão geral

O sistema atende ao fluxo de cadastro de produtos, criação de notas fiscais e impressão de notas. A solução foi organizada em dois microsserviços de negócio, cada um com seu próprio banco PostgreSQL:

- **Estoque**: cadastra produtos e controla os saldos disponíveis.
- **Faturamento**: cria, consulta e fecha notas fiscais, além de disponibilizar o PDF.

O projeto `SistemaEmissaoNF.Faturamento.Worker` é um processo separado do contexto de Faturamento. Ele consome mensagens de background e gera os PDFs.

## 2. Arquitetura dos microsserviços

Cada microsserviço segue uma separação em camadas:

- **Domain**: entidades e regras do domínio.
- **Application**: casos de uso, commands, queries, validações e contratos.
- **Infrastructure**: Entity Framework Core, repositories, banco de dados e integrações externas.
- **API**: endpoints HTTP, controllers e middleware.

O Faturamento não acessa o DbContext ou o repository do Estoque. A comunicação entre os serviços acontece por HTTP, preservando a autonomia dos bancos e dos domínios.

## 3. Tecnologias e frameworks

### Backend

- C# com .NET 10.
- ASP.NET Core Web API para os microsservicos.
- Worker Service para processamento em background.
- MediatR para o padrão CQRS e encaminhamento de commands e queries.
- Entity Framework Core com PostgreSQL para persistência.
- FluentValidation para validação dos comandos.
- Mapster para mapeamentos entre entidades e responses.
- MassTransit e RabbitMQ para mensageria assíncrona.
- QuestPDF para geração dos arquivos PDF.
- Swagger/OpenAPI para documentação e teste dos endpoints.

### Frontend

- Angular 20 com standalone components.
- Angular Material e Angular CDK para os componentes visuais e interações.
- Reactive Forms para formulários e validação.
- HttpClient para comunicacao com as APIs.
- RxJS 7 para composição e controle dos fluxos assíncronos.

As dependências .NET são declaradas nos arquivos `.csproj` por `PackageReference`; as dependências Angular/JavaScript são declaradas no `package.json` e fixadas pelo `package-lock.json`.

## 4. Angular: ciclo de vida e RxJS

O ciclo de vida utilizado explicitamente nos componentes é `ngOnInit`, por meio da interface `OnInit`. Ele é usado para carregar dados iniciais, como a listagem de produtos e notas fiscais, e para preparar os formulários.

A aplicação utiliza RxJS principalmente no fluxo de impressão da NF. Depois que a API fecha a nota, o frontend consulta periodicamente o endpoint do PDF usando:

- `timer`: inicia as consultas em intervalos.
- `switchMap`: transforma cada disparo em uma requisição HTTP.
- `takeWhile`: continua enquanto o PDF ainda não estiver pronto.
- `filter` e `take`: selecionam e encerram o fluxo quando o PDF está disponível.
- `finalize`: encerra o indicador de processamento em qualquer resultado.

Os observables do HttpClient também são usados para carregar listas, criar produtos, criar notas e exibir o resultado das operações.

## 5. CQRS e casos de uso

As APIs recebem HTTP e delegam o processamento ao MediatR. Commands alteram estado, por exemplo:

- criar produto;
- criar nota fiscal;
- imprimir/fechar nota fiscal;
- baixar saldo do estoque.

Queries somente consultam dados, como listar produtos, listar notas, buscar detalhes e obter o PDF. Essa separação mantém os controllers pequenos e concentra as regras nos handlers da camada Application.

## 6. Persistência, EF Core e LINQ

O Entity Framework Core utiliza Fluent API para configurar tabelas, chaves, índices únicos, relacionamentos, tipos e a sequence de numeração. O Estoque utiliza o banco `estoque_db` e o Faturamento utiliza o banco `faturamento_db`.

Não existem foreign keys entre Produto e NotaFiscalItem, pois Produto pertence ao microsserviço Estoque. O item da NF armazena os dados necessários para o contexto de Faturamento.

LINQ foi utilizado no backend para consultar e transformar coleções, incluindo:

- `Select` para projetar entidades em responses e montar itens de requisições;
- `Where` para filtrar resultados de validação;
- `Include` para carregar os itens relacionados da nota fiscal;
- `OrderByDescending` para ordenar notas por número;
- `ToListAsync` para materializar consultas de forma assíncrona.

## 7. Numeração sequencial

O Faturamento utiliza a sequence PostgreSQL `nota_fiscal_numero_seq`. O número é gerado no backend e não no Angular. Assim, a aplicação não depende de `MAX(numero) + 1`, evitando colisão em criações concorrentes simples.

## 8. Impressão e comunicação entre serviços

Ao imprimir uma NF, o Faturamento valida se ela está `Aberta` e chama `POST /api/estoque/baixar` usando `IHttpClientFactory`. O Estoque baixa os saldos em sua própria transação local. Somente após a baixa bem-sucedida o Faturamento fecha a nota.

Se o Estoque estiver indisponível ou rejeitar a baixa, o Faturamento retorna `503 Service Unavailable` e a NF permanece aberta. Isso permite corrigir a falha e repetir a operação sem fechar uma nota cuja baixa não foi confirmada.

Depois de fechar a NF, o Faturamento publica `GerarNotaFiscalPdfCommand` na fila `gerar-nota-fiscal-pdf`. O Worker consome a mensagem, carrega a nota, gera o PDF com QuestPDF e atualiza `StatusImpressao`, `PdfArquivo` e `PdfGeradoEm`.

Se o Worker estiver parado, a mensagem permanece no RabbitMQ e é processada quando o Worker voltar.

## 9. Tratamento de erros e exceções

As APIs possuem middleware centralizado de exceções. As falhas são convertidas em respostas HTTP consistentes, conforme o caso:

- `400 Bad Request`: dados inválidos ou falha de validação;
- `404 Not Found`: produto ou nota não encontrado;
- `503 Service Unavailable`: indisponibilidade ou falha na comunicação com o Estoque;
- `500 Internal Server Error`: erro inesperado não tratado.

No frontend, as respostas de erro são apresentadas ao usuário por snackbar. Durante a impressão, a tela também exibe indicador de processamento e informa que a nota permanece aberta quando o Estoque falha.

## 10. PDF e armazenamento

O PDF é persistido no PostgreSQL como `byte[]`, mantendo o projeto autocontido para o teste técnico. O endpoint de PDF retorna o arquivo quando a geração assíncrona termina.

Em um ambiente produtivo de alto volume, o armazenamento poderia ser substituído por object storage, como Amazon S3 ou Azure Blob Storage, mantendo no banco apenas os metadados e a referência do arquivo.

## 11. Testes

Os testes unitários cobrem os principais casos de uso, incluindo:

- validações de comandos;
- cadastro e duplicidade de produtos;
- baixa atômica de estoque;
- criação e impressão de NF;
- tentativa de impressão com Estoque indisponível;
- geração de PDF sem depender de um RabbitMQ real.

## 12. Execução local

As instruções completas para subir PostgreSQL, RabbitMQ, as APIs, o Worker e o Angular estão no [README.md](../README.md). O fluxo demonstrado é: cadastrar produtos, criar uma NF aberta, imprimir, baixar o estoque, fechar a NF, gerar o PDF no Worker e consultá-lo pelo frontend.