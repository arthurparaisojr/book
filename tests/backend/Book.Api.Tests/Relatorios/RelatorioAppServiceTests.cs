using Book.Application.Abstractions.Persistence;
using Book.Application.Contracts.Relatorios;
using Book.Application.Exceptions;
using Book.Application.Services.Relatorios;

namespace Book.Api.Tests.Relatorios;

public sealed class RelatorioAppServiceTests
{
    [Fact]
    public async Task ListLivrosPorAutorAsync_ShouldReturnRows_WhenRepositoryHasData()
    {
        var repository = new FakeRelatorioRepository();
        repository.Seed(new RelatorioLivroPorAutorResponse
        {
            CodAu = 1,
            AutorNome = "Martin Fowler",
            Codl = 10,
            Titulo = "Refactoring",
            Editora = "Addison-Wesley",
            Edicao = 2,
            AnoPublicacao = "2019",
            Valor = 199.90m,
            Assuntos = "Refactoring, Design"
        });

        var service = new RelatorioAppService(repository);

        var response = await service.ListLivrosPorAutorAsync(new ListRelatorioLivrosPorAutorRequest());

        Assert.Single(response);
        Assert.Equal("Martin Fowler", response[0].AutorNome);
        Assert.Equal("Refactoring", response[0].Titulo);
    }

    [Fact]
    public async Task ListLivrosPorAutorAsync_ShouldFilterByAutorNome_WhenParameterIsProvided()
    {
        var repository = new FakeRelatorioRepository();
        repository.Seed(new RelatorioLivroPorAutorResponse
        {
            CodAu = 1,
            AutorNome = "Martin Fowler",
            Codl = 10,
            Titulo = "Refactoring",
            Editora = "Addison-Wesley",
            Edicao = 2,
            AnoPublicacao = "2019",
            Valor = 199.90m,
            Assuntos = "Refactoring"
        });
        repository.Seed(new RelatorioLivroPorAutorResponse
        {
            CodAu = 2,
            AutorNome = "Eric Evans",
            Codl = 11,
            Titulo = "Domain-Driven Design",
            Editora = "Pearson",
            Edicao = 1,
            AnoPublicacao = "2003",
            Valor = 189.90m,
            Assuntos = "DDD"
        });

        var service = new RelatorioAppService(repository);

        var response = await service.ListLivrosPorAutorAsync(new ListRelatorioLivrosPorAutorRequest
        {
            AutorNome = "Martin"
        });

        Assert.Single(response);
        Assert.Equal("Martin Fowler", response[0].AutorNome);
    }

    [Fact]
    public async Task ListLivrosPorAutorAsync_ShouldThrowValidationException_WhenAutorNomeIsTooLong()
    {
        var repository = new FakeRelatorioRepository();
        var service = new RelatorioAppService(repository);

        var action = async () => await service.ListLivrosPorAutorAsync(new ListRelatorioLivrosPorAutorRequest
        {
            AutorNome = new string('a', 41)
        });

        await Assert.ThrowsAsync<ValidationException>(action);
    }

    private sealed class FakeRelatorioRepository : IRelatorioRepository
    {
        private readonly List<RelatorioLivroPorAutorResponse> _rows = [];

        public Task<IReadOnlyList<RelatorioLivroPorAutorResponse>> ListLivrosPorAutorAsync(
            ListRelatorioLivrosPorAutorRequest request,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<RelatorioLivroPorAutorResponse> query = _rows;

            if (!string.IsNullOrWhiteSpace(request.AutorNome))
            {
                query = query.Where(row =>
                    row.AutorNome.Contains(request.AutorNome, StringComparison.OrdinalIgnoreCase));
            }

            return Task.FromResult<IReadOnlyList<RelatorioLivroPorAutorResponse>>(query.ToArray());
        }

        public void Seed(RelatorioLivroPorAutorResponse row)
        {
            _rows.Add(row);
        }
    }
}
