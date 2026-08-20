using SistemaEmissaoNF.Faturamento.Application;
using SistemaEmissaoNF.Faturamento.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddFaturamentoApplication();
builder.Services.AddFaturamentoInfrastructure(builder.Configuration, addWorkerConsumer: true);

var host = builder.Build();
host.Run();
