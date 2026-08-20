# Sistema de Emissao de Notas Fiscais

Aplicacao de teste tecnico com Angular 20 no frontend, C#/.NET 10 no backend, dois microsservicos de negocio, PostgreSQL, HTTP entre servicos, RabbitMQ e Worker para geracao assincrona de PDF.

## Arquitetura

- `Estoque.Api`: cadastra produtos e controla saldo.
- `Faturamento.Api`: cria, lista, consulta, imprime e disponibiliza PDF de notas fiscais.
- `Faturamento.Worker`: processo de background do contexto de Faturamento que consome RabbitMQ e gera PDF.
- `estoque_db`: banco exclusivo do Estoque.
- `faturamento_db`: banco exclusivo do Faturamento.

Faturamento nao acessa banco, DbContext ou repository de Estoque. A baixa de estoque acontece por HTTP.

## Por Que HTTP e RabbitMQ

HTTP e usado na baixa de estoque porque Faturamento precisa saber imediatamente se o saldo foi baixado antes de fechar a nota.

RabbitMQ e usado na geracao do PDF porque o PDF e um processamento secundario. A API fecha a NF, publica uma mensagem pequena na fila `gerar-nota-fiscal-pdf` e o Worker processa em background.

Se o Worker estiver temporariamente indisponivel, a mensagem permanece na fila e sera processada quando ele voltar.

## Como Executar

Suba a infraestrutura:

```bash
docker compose up -d
```

Aplicar migrations:

```bash
dotnet ef database update -p backend/SistemaEmissaoNF.Estoque.Infrastructure -s backend/SistemaEmissaoNF.Estoque.Api
dotnet ef database update -p backend/SistemaEmissaoNF.Faturamento.Infrastructure -s backend/SistemaEmissaoNF.Faturamento.Api
```

Subir Estoque:

```bash
dotnet run --project backend/SistemaEmissaoNF.Estoque.Api --launch-profile http
```

Subir Faturamento:

```bash
dotnet run --project backend/SistemaEmissaoNF.Faturamento.Api --launch-profile http
```

Subir Worker:

```bash
dotnet run --project backend/SistemaEmissaoNF.Faturamento.Worker
```

Subir Angular:

```bash
cd frontend/sistema-emissao-nf
npm install
npm start
```

URLs locais:

- Angular: `http://localhost:4200`
- Estoque API: `http://localhost:5153/swagger`
- Faturamento API: `http://localhost:5198/swagger`
- RabbitMQ Management: `http://localhost:15672` (`guest` / `guest`)
- pgAdmin: `http://localhost:5050` (`admin@local.com` / `admin`)

## Testes e Build

```bash
dotnet build SistemaEmissaoNF.sln
dotnet test SistemaEmissaoNF.sln
cd frontend/sistema-emissao-nf
npm run build
```

## Fluxo Principal

1. Cadastre produtos em `Produtos`.
2. Crie uma NF em `Notas Fiscais > Incluir`.
3. A NF nasce `Aberta` com numero sequencial gerado pelo backend.
4. Clique em imprimir.
5. Faturamento chama Estoque por HTTP.
6. Estoque baixa saldo em transacao local.
7. Faturamento fecha a NF.
8. Faturamento publica `GerarNotaFiscalPdfCommand` no RabbitMQ.
9. Worker consome a mensagem, gera o PDF e salva em `faturamento_db`.
10. Angular faz polling e abre o PDF quando receber `200 OK`.

## Cenario de Falha do Estoque

1. Crie uma NF aberta.
2. Pare `SistemaEmissaoNF.Estoque.Api`.
3. Clique em imprimir no Angular.
4. Faturamento retornara `503 Service Unavailable`.
5. A NF permanecera Aberta.
6. Suba o Estoque novamente e clique em imprimir outra vez.

Mensagem esperada no frontend:

```text
Nao foi possivel comunicar com o servico de estoque.
A Nota Fiscal permanece aberta.
Tente novamente.
```

## Demonstracao RabbitMQ

1. Mantenha RabbitMQ, Estoque e Faturamento ativos.
2. Pare `SistemaEmissaoNF.Faturamento.Worker`.
3. Imprima uma NF.
4. A NF sera fechada e a mensagem ficara aguardando na fila `gerar-nota-fiscal-pdf`.
5. Inicie o Worker.
6. O Worker consumira a mensagem e gerara o PDF.

## PDF

Para simplificar a execucao local do teste tecnico, o PDF foi persistido no PostgreSQL. Em um ambiente produtivo com grande volume de arquivos, seria preferivel utilizar um servico de object storage.

## Tecnologias

- .NET 10, ASP.NET Core Web API, Worker Service
- Entity Framework Core, PostgreSQL, LINQ
- CQRS, MediatR, Mapster, FluentValidation
- HttpClient/IHttpClientFactory
- MassTransit + RabbitMQ
- QuestPDF
- Angular 20, Standalone Components, Reactive Forms, Angular Material, RxJS
