using System.Linq.Expressions;
using Mapster;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SistemaEmissaoNF.Estoque.Application.Interfaces;
using SistemaEmissaoNF.Estoque.Application.Services;
using SistemaEmissaoNF.Estoque.Application.UseCases.Estoque.Commands.Baixar;
using SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Commands.Create;
using SistemaEmissaoNF.Estoque.Application.UseCases.Produto.Commands.Update;
using SistemaEmissaoNF.Estoque.Domain.Entities;
using SistemaEmissaoNF.Estoque.Infrastructure.Context;
using SistemaEmissaoNF.Estoque.Infrastructure.Repositories;

namespace SistemaEmissaoNF.Estoque.Tests;

public class ProdutoUseCasesTests
{
    [Fact]
    public async Task CreateProduto_DeveCriarProdutoValido()
    {
        var repository = new FakeProdutoRepository();
        var handler = new CreateProdutoCommandHandler(repository, new Mapper(TypeAdapterConfig.GlobalSettings), new CreateProdutoCommandValidator());

        var response = await handler.Handle(new CreateProdutoCommand
        {
            Codigo = 1001,
            Descricao = "Produto A",
            Saldo = 10
        }, CancellationToken.None);

        Assert.True(response.Success);
        Assert.Equal(1001, response.Data!.Codigo);
        Assert.Single(repository.Produtos);
    }

    [Theory]
    [InlineData(0, "Produto", 1, "Informe um codigo valido.")]
    [InlineData(1, "", 1, "Informe a descricao do produto.")]
    [InlineData(1, "Produto", -1, "O saldo nao pode ser negativo.")]
    public async Task CreateProduto_DeveValidarCampos(long codigo, string descricao, int saldo, string erroEsperado)
    {
        var handler = new CreateProdutoCommandHandler(
            new FakeProdutoRepository(),
            new Mapper(TypeAdapterConfig.GlobalSettings),
            new CreateProdutoCommandValidator());

        var response = await handler.Handle(new CreateProdutoCommand
        {
            Codigo = codigo,
            Descricao = descricao,
            Saldo = saldo
        }, CancellationToken.None);

        Assert.False(response.Success);
        Assert.Contains(erroEsperado, response.Errors);
    }

    [Fact]
    public async Task CreateProduto_DeveBloquearCodigoDuplicado()
    {
        var repository = new FakeProdutoRepository(new Produto { Id = 1, Codigo = 1001, Descricao = "Produto A", Saldo = 5 });
        var handler = new CreateProdutoCommandHandler(repository, new Mapper(TypeAdapterConfig.GlobalSettings), new CreateProdutoCommandValidator());

        var response = await handler.Handle(new CreateProdutoCommand
        {
            Codigo = 1001,
            Descricao = "Produto B",
            Saldo = 3
        }, CancellationToken.None);

        Assert.False(response.Success);
        Assert.Contains("Ja existe um produto cadastrado com este codigo.", response.Errors);
    }

    [Fact]
    public async Task UpdateProduto_DeveAlterarProduto()
    {
        var repository = new FakeProdutoRepository(new Produto { Id = 1, Codigo = 1001, Descricao = "Produto A", Saldo = 5 });
        var handler = new UpdateProdutoCommandHandler(repository, new Mapper(TypeAdapterConfig.GlobalSettings), new UpdateProdutoCommandValidator());

        var response = await handler.Handle(new UpdateProdutoCommand
        {
            Id = 1,
            Codigo = 1002,
            Descricao = "Produto Alterado",
            Saldo = 8
        }, CancellationToken.None);

        Assert.True(response.Success);
        Assert.Equal(1002, repository.Produtos.Single().Codigo);
        Assert.Equal("Produto Alterado", repository.Produtos.Single().Descricao);
        Assert.Equal(8, repository.Produtos.Single().Saldo);
    }

    [Fact]
    public async Task BaixarEstoque_DeveBaixarMultiplosItens()
    {
        await using var dbContext = await CreateDbContextAsync();
        dbContext.Produtos.AddRange(
            new Produto { Codigo = 1001, Descricao = "Produto A", Saldo = 10 },
            new Produto { Codigo = 1002, Descricao = "Produto B", Saldo = 5 });
        await dbContext.SaveChangesAsync();
        var repository = new ProdutoRepository(dbContext);

        var errors = await repository.BaixarEstoqueAsync([
            new BaixaEstoqueItemRequest { ProdutoId = 1, Quantidade = 2 },
            new BaixaEstoqueItemRequest { ProdutoId = 2, Quantidade = 3 }
        ], CancellationToken.None);

        Assert.Empty(errors);
        Assert.Equal(8, (await dbContext.Produtos.FindAsync(1))!.Saldo);
        Assert.Equal(2, (await dbContext.Produtos.FindAsync(2))!.Saldo);
    }

    [Fact]
    public async Task BaixarEstoque_QuandoUmItemFalha_NaoAlteraOsDemais()
    {
        await using var dbContext = await CreateDbContextAsync();
        dbContext.Produtos.AddRange(
            new Produto { Codigo = 1001, Descricao = "Produto A", Saldo = 10 },
            new Produto { Codigo = 1002, Descricao = "Produto B", Saldo = 1 });
        await dbContext.SaveChangesAsync();
        var repository = new ProdutoRepository(dbContext);

        var errors = await repository.BaixarEstoqueAsync([
            new BaixaEstoqueItemRequest { ProdutoId = 1, Quantidade = 2 },
            new BaixaEstoqueItemRequest { ProdutoId = 2, Quantidade = 3 }
        ], CancellationToken.None);

        Assert.NotEmpty(errors);
        Assert.Equal(10, (await dbContext.Produtos.FindAsync(1))!.Saldo);
        Assert.Equal(1, (await dbContext.Produtos.FindAsync(2))!.Saldo);
    }

    private static async Task<EstoqueDbContext> CreateDbContextAsync()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<EstoqueDbContext>()
            .UseSqlite(connection)
            .Options;
        var dbContext = new EstoqueDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();
        return dbContext;
    }

    private sealed class FakeProdutoRepository(params Produto[] produtos) : IProdutoRepository
    {
        public List<Produto> Produtos { get; } = produtos.ToList();

        public Task<Produto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Produtos.FirstOrDefault(x => x.Id == id));
        }

        public Task<Produto?> FirstOrDefaultAsync(Expression<Func<Produto, bool>> predicate, CancellationToken cancellationToken)
        {
            return Task.FromResult(Produtos.AsQueryable().FirstOrDefault(predicate));
        }

        public Task<List<Produto>> ListAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            return Task.FromResult(Produtos.Skip((page - 1) * pageSize).Take(pageSize).ToList());
        }

        public Task<int> CountAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Produtos.Count);
        }

        public Task InsertAsync(Produto entity, CancellationToken cancellationToken)
        {
            entity.Id = Produtos.Count == 0 ? 1 : Produtos.Max(x => x.Id) + 1;
            Produtos.Add(entity);
            return Task.CompletedTask;
        }

        public void Update(Produto entity)
        {
        }

        public Task SaveAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<bool> CodigoExistsAsync(long codigo, int? ignoreId, CancellationToken cancellationToken)
        {
            return Task.FromResult(Produtos.Any(x => x.Codigo == codigo && (!ignoreId.HasValue || x.Id != ignoreId.Value)));
        }

        public Task<List<string>> BaixarEstoqueAsync(IReadOnlyCollection<BaixaEstoqueItemRequest> itens, CancellationToken cancellationToken)
        {
            return Task.FromResult(new List<string>());
        }
    }
}
