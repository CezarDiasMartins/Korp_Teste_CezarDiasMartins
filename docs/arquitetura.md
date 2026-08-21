# Documentação Técnica

## Arquitetura

O sistema possui dois microsserviços de negócio: Estoque e Faturamento. Cada um tem seu proprio dominio, Application, Infrastructure, API e banco PostgreSQL. O projeto `Faturamento.Worker` não é um terceiro microsserviço; ele e outro processo do contexto de Faturamento usado para tarefas de background.

## .NET, CQRS e Mediator

As APIs ASP.NET Core recebem HTTP, montam Commands ou Queries e delegam para o MediatR. Commands alteram estado, como criar produto, criar NF ou imprimir NF. Queries consultam dados, como listar produtos ou buscar PDF. Isso deixa Controllers finas e concentra regras nos handlers.

## Entity Framework Core, LINQ e PostgreSQL

O EF Core usa Fluent API para mapear tabelas, chaves, indices unicos, relacionamentos e tipos `char(1)` dos status. O Estoque grava em `estoque_db`; Faturamento grava em `faturamento_db`. Não éxistem foreign keys entre Produto e NotaFiscalItem porque Produto pertence ao microsserviço Estoque.

## Numeração Sequencial

Faturamento usa a sequence PostgreSQL `nota_fiscal_numero_seq`. O número não é calculado no Angular e não depende de `MAX(numero) + 1`, evitando duplicidade em criações concorrentes simples.

## HttpClient

Faturamento usa `IHttpClientFactory` para chamar `POST /api/estoque/baixar`. Esse fluxo e sincrono porque a API precisa saber imediatamente se a baixa ocorreu antes de fechar a NF. Se Estoque estiver offline, Faturamento retorna 503 e a NF permanece Aberta.

## RabbitMQ e Worker Service

Apos fechar a NF, Faturamento publica `GerarNotaFiscalPdfCommand` na fila `gerar-nota-fiscal-pdf`. O Worker consome a mensagem, carrega a NF no `faturamento_db`, gera o PDF com QuestPDF e atualiza `StatusImpressao`, `PdfArquivo` e `PdfGeradoEm`.

Se o Worker estiver parado, a mensagem fica aguardando no RabbitMQ. Quando ele volta, a fila e consumida e o PDF e gerado.

## Mapster e FluentValidation

Mapster e usado para mapeamentos simples de entidade para response. FluentValidation valida comandos com mensagens em Portugues do Brasil.

## Angular, Material e RxJS

O frontend usa Angular 20 com standalone components, Angular Material, Reactive Forms e HttpClient. Depois de imprimir uma NF, o frontend usa `timer`, `switchMap`, `takeWhile`, `filter`, `take` e `finalize` para consultar o endpoint de PDF em intervalos simples ate receber `200 OK`.

## Tratamento de Erros

As APIs possuem middleware centralizado de exceções. Controllers convertem responses em HTTP adequado: 400, 404, 503 e 500. O frontend mostra feedback via snackbar.

## PDF

O PDF fica salvo no PostgreSQL como `byte[]` para manter o teste autocontido. Em produção com alto volume, o ideal seria object storage como S3, Azure Blob ou equivalente.

## Testes

Os testes unitários cobrem validações, duplicidade, baixa atômica, criação/impressão de NF, falha do Estoque e geração de PDF sem RabbitMQ real.
