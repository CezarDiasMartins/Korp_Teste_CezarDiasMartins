using System.Linq.Expressions;
using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;
using SistemaEmissaoNF.Faturamento.Application.Behaviors;
using SistemaEmissaoNF.Faturamento.Application.Interfaces;
using SistemaEmissaoNF.Faturamento.Application.Messaging;
using SistemaEmissaoNF.Faturamento.Application.Response;
using SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Commands.Create;
using SistemaEmissaoNF.Faturamento.Application.UseCases.NotaFiscal.Commands.Print;
using SistemaEmissaoNF.Faturamento.Domain.Entities;
using SistemaEmissaoNF.Faturamento.Domain.Enums;
using SistemaEmissaoNF.Faturamento.Infrastructure.Services;

namespace SistemaEmissaoNF.Faturamento.Tests;

public class NotaFiscalUseCasesTests
{
    [Fact]
    public async Task CreateNotaFiscal_DeveCriarAberta()
    {
        var repository = new FakeNotaFiscalRepository();
        var handler = new CreateNotaFiscalCommandHandler(repository);

        var response = await handler.Handle(CreateCommand(), CancellationToken.None);

        Assert.True(response.Success);
        Assert.Equal("A", response.Data!.Status);
        Assert.Equal(1, response.Data.NumeroSequencial);
        Assert.Single(repository.Notas);
    }

    [Fact]
    public async Task CreateNotaFiscal_DeveBloquearNotaSemItens()
    {
        var handler = new CreateNotaFiscalCommandHandler(new FakeNotaFiscalRepository());

        var response = await ValidateAsync<CreateNotaFiscalCommand, GenericDataResponse<Application.UseCases.NotaFiscal.Response.NotaFiscalResponse>>(new CreateNotaFiscalCommand(), [new CreateNotaFiscalCommandValidator()]);

        Assert.False(response.Success);
        Assert.Contains("Informe ao menos um item para a nota fiscal.", response.Errors);
    }

    [Fact]
    public async Task CreateNotaFiscal_DeveBloquearQuantidadeInvalida()
    {
        var handler = new CreateNotaFiscalCommandHandler(new FakeNotaFiscalRepository());
        var command = CreateCommand();
        command.Itens[0].Quantidade = 0;

        var response = await ValidateAsync<CreateNotaFiscalCommand, GenericDataResponse<Application.UseCases.NotaFiscal.Response.NotaFiscalResponse>>(command, [new CreateNotaFiscalCommandValidator()]);

        Assert.False(response.Success);
        Assert.Contains("Informe uma quantidade válida.", response.Errors);
    }

    [Fact]
    public async Task CreateNotaFiscal_DeveBloquearProdutoDuplicado()
    {
        var handler = new CreateNotaFiscalCommandHandler(new FakeNotaFiscalRepository());
        var command = CreateCommand();
        command.Itens.Add(new CreateNotaFiscalItemCommand
        {
            ProdutoId = 1,
            ProdutoCodigo = 1001,
            ProdutoDescricao = "Produto A",
            Quantidade = 1
        });

        var response = await handler.Handle(command, CancellationToken.None);

        Assert.False(response.Success);
        Assert.Contains("Não é permitido informar o mesmo produto mais de uma vez.", response.Errors);
    }

    [Fact]
    public async Task PrintNotaFiscal_DeveBloquearNotaFechada()
    {
        var nota = NotaAberta();
        nota.Status = StatusNotaFiscal.Fechada;
        var handler = new PrintNotaFiscalCommandHandler(new FakeNotaFiscalRepository(nota), new FakeEstoqueService(), new FakePdfPublisher());

        var response = await handler.Handle(new PrintNotaFiscalCommand { Id = 1 }, CancellationToken.None);

        Assert.False(response.Success);
        Assert.Contains("Nota fiscal já está fechada.", response.Errors);
    }

    [Fact]
    public async Task PrintNotaFiscal_QuandoEstoqueFalha_DeveManterAberta()
    {
        var nota = NotaAberta();
        var estoqueService = new FakeEstoqueService(new BaixaEstoqueResponse
        {
            ServiceUnavailable = true,
            Errors = ["Não foi possível comunicar com o serviço de estoque."]
        });
        var handler = new PrintNotaFiscalCommandHandler(new FakeNotaFiscalRepository(nota), estoqueService, new FakePdfPublisher());

        var response = await handler.Handle(new PrintNotaFiscalCommand { Id = 1 }, CancellationToken.None);

        Assert.False(response.Success);
        Assert.True(response.ServiceUnavailable);
        Assert.Equal(StatusNotaFiscal.Aberta, nota.Status);
    }

    [Fact]
    public async Task PrintNotaFiscal_DeveFecharNotaEPublicarPdf()
    {
        var nota = NotaAberta();
        var publisher = new FakePdfPublisher();
        var handler = new PrintNotaFiscalCommandHandler(new FakeNotaFiscalRepository(nota), new FakeEstoqueService(), publisher);

        var response = await handler.Handle(new PrintNotaFiscalCommand { Id = 1 }, CancellationToken.None);

        Assert.True(response.Success);
        Assert.Equal(StatusNotaFiscal.Fechada, nota.Status);
        Assert.Single(publisher.Commands);
    }

    [Fact]
    public async Task NotaFiscalPdfService_DeveGerarPdf()
    {
        var nota = NotaAberta();
        nota.Status = StatusNotaFiscal.Fechada;
        var repository = new FakeNotaFiscalRepository(nota);
        var service = new NotaFiscalPdfService(repository, NullLogger<NotaFiscalPdfService>.Instance);

        await service.ProcessarPdfAsync(1, CancellationToken.None);

        Assert.Equal(StatusImpressao.Concluido, nota.StatusImpressao);
        Assert.NotNull(nota.PdfArquivo);
        Assert.True(nota.PdfArquivo!.Length > 0);
    }

    private static async Task<TResponse> ValidateAsync<TRequest, TResponse>(TRequest request, IEnumerable<IValidator<TRequest>> validators)
        where TRequest : notnull
        where TResponse : IResponse, new()
    {
        var nextWasCalled = false;
        var behavior = new ValidationBehavior<TRequest, TResponse>(validators);
        var response = await behavior.Handle(request, _ =>
        {
            nextWasCalled = true;
            return Task.FromResult(new TResponse());
        }, CancellationToken.None);

        Assert.False(nextWasCalled);
        return response;
    }
    private static CreateNotaFiscalCommand CreateCommand()
    {
        return new CreateNotaFiscalCommand
        {
            Itens =
            [
                new CreateNotaFiscalItemCommand
                {
                    ProdutoId = 1,
                    ProdutoCodigo = 1001,
                    ProdutoDescricao = "Produto A",
                    Quantidade = 2
                }
            ]
        };
    }

    private static NotaFiscal NotaAberta()
    {
        return new NotaFiscal
        {
            Id = 1,
            NumeroSequencial = 1,
            Status = StatusNotaFiscal.Aberta,
            StatusImpressao = StatusImpressao.Pendente,
            Itens =
            [
                new NotaFiscalItem
                {
                    Id = 1,
                    NotaFiscalId = 1,
                    ProdutoId = 1,
                    ProdutoCodigo = 1001,
                    ProdutoDescricao = "Produto A",
                    Quantidade = 2
                }
            ]
        };
    }

    private sealed class FakeNotaFiscalRepository(params NotaFiscal[] notas) : INotaFiscalRepository
    {
        private long _nextNumber = 1;

        public List<NotaFiscal> Notas { get; } = notas.ToList();

        public Task<long> GetNextNumeroSequencialAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_nextNumber++);
        }

        public Task<NotaFiscal?> GetWithItensAsync(int id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Notas.FirstOrDefault(x => x.Id == id));
        }

        public Task<List<NotaFiscal>> ListWithItensAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            return Task.FromResult(Notas.Skip((page - 1) * pageSize).Take(pageSize).ToList());
        }

        public Task<NotaFiscal?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Notas.FirstOrDefault(x => x.Id == id));
        }

        public Task<NotaFiscal?> FirstOrDefaultAsync(Expression<Func<NotaFiscal, bool>> predicate, CancellationToken cancellationToken)
        {
            return Task.FromResult(Notas.AsQueryable().FirstOrDefault(predicate));
        }

        public Task<List<NotaFiscal>> ListAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            return Task.FromResult(Notas.Skip((page - 1) * pageSize).Take(pageSize).ToList());
        }

        public Task<int> CountAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Notas.Count);
        }

        public Task InsertAsync(NotaFiscal entity, CancellationToken cancellationToken)
        {
            entity.Id = Notas.Count == 0 ? 1 : Notas.Max(x => x.Id) + 1;
            Notas.Add(entity);
            return Task.CompletedTask;
        }

        public void Update(NotaFiscal entity)
        {
        }

        public Task SaveAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakeEstoqueService(BaixaEstoqueResponse? response = null) : IEstoqueService
    {
        public Task<BaixaEstoqueResponse> BaixarEstoqueAsync(BaixaEstoqueRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(response ?? new BaixaEstoqueResponse());
        }
    }

    private sealed class FakePdfPublisher : INotaFiscalPdfPublisher
    {
        public List<GerarNotaFiscalPdfCommand> Commands { get; } = [];

        public Task PublishAsync(GerarNotaFiscalPdfCommand command, CancellationToken cancellationToken)
        {
            Commands.Add(command);
            return Task.CompletedTask;
        }
    }
}
