using Book.Application.Contracts.Relatorios;
using Book.Application.Exceptions;
using Book.Application.Services.Relatorios;

namespace Book.Api.Tests.Relatorios;

public sealed class RelatorioExportAppServiceTests
{
    [Fact]
    public async Task ExportLivrosPorAutorPdfAsync_ShouldCombineQueryAndPdfServices()
    {
        var fakeNow = new DateTimeOffset(2026, 04, 26, 14, 30, 00, TimeSpan.FromHours(-3));
        RelatorioLivroPorAutorResponse[] relatorioRows =
        [
            new RelatorioLivroPorAutorResponse
            {
                CodAu = 1,
                AutorNome = "Martin Fowler",
                Codl = 10,
                Titulo = "Refactoring",
                Editora = "Addison-Wesley",
                Edicao = 2,
                AnoPublicacao = "2019",
                Valor = 199.90m,
                Assuntos = "Design, Refactoring"
            }
        ];

        var relatorioAppService = new FakeRelatorioAppService(relatorioRows);
        var relatorioPdfService = new FakeRelatorioPdfService(
            new RelatorioPdfResponse
            {
                NomeArquivo = "relatorio.pdf",
                TipoConteudo = "application/pdf",
                Conteudo = [1, 2, 3]
            });
        var service = new RelatorioExportAppService(
            relatorioAppService,
            relatorioPdfService,
            new FakeTimeProvider(fakeNow));

        var response = await service.ExportLivrosPorAutorPdfAsync(new ListRelatorioLivrosPorAutorRequest
        {
            AutorNome = "Martin"
        });

        Assert.Equal("relatorio.pdf", response.NomeArquivo);
        Assert.Equal("application/pdf", response.TipoConteudo);
        Assert.Equal([1, 2, 3], response.Conteudo);
        Assert.Equal("Martin", relatorioAppService.LastRequest?.AutorNome);
        Assert.Equal("Martin", relatorioPdfService.LastRequest?.AutorNomeFiltro);
        Assert.Equal(fakeNow, relatorioPdfService.LastRequest?.GeradoEm);
        Assert.Single(relatorioPdfService.LastRequest?.Itens ?? []);
    }

    [Fact]
    public async Task ExportLivrosPorAutorPdfAsync_ShouldPropagateValidationFailureFromQueryService()
    {
        var service = new RelatorioExportAppService(
            new ThrowingRelatorioAppService(),
            new FakeRelatorioPdfService(new RelatorioPdfResponse()),
            new FakeTimeProvider(DateTimeOffset.Now));

        var action = async () => await service.ExportLivrosPorAutorPdfAsync(new ListRelatorioLivrosPorAutorRequest
        {
            AutorNome = new string('x', 41)
        });

        await Assert.ThrowsAsync<ValidationException>(action);
    }

    private sealed class FakeRelatorioAppService : IRelatorioAppService
    {
        private readonly IReadOnlyList<RelatorioLivroPorAutorResponse> _rows;

        public FakeRelatorioAppService(IReadOnlyList<RelatorioLivroPorAutorResponse> rows)
        {
            _rows = rows;
        }

        public ListRelatorioLivrosPorAutorRequest? LastRequest { get; private set; }

        public Task<IReadOnlyList<RelatorioLivroPorAutorResponse>> ListLivrosPorAutorAsync(
            ListRelatorioLivrosPorAutorRequest request,
            CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.FromResult(_rows);
        }
    }

    private sealed class ThrowingRelatorioAppService : IRelatorioAppService
    {
        public Task<IReadOnlyList<RelatorioLivroPorAutorResponse>> ListLivrosPorAutorAsync(
            ListRelatorioLivrosPorAutorRequest request,
            CancellationToken cancellationToken = default)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                [nameof(ListRelatorioLivrosPorAutorRequest.AutorNome)] = ["Filtro invalido."]
            });
        }
    }

    private sealed class FakeRelatorioPdfService : IRelatorioPdfService
    {
        private readonly RelatorioPdfResponse _response;

        public FakeRelatorioPdfService(RelatorioPdfResponse response)
        {
            _response = response;
        }

        public GerarRelatorioLivrosPorAutorPdfRequest? LastRequest { get; private set; }

        public Task<RelatorioPdfResponse> GerarRelatorioLivrosPorAutorAsync(
            GerarRelatorioLivrosPorAutorPdfRequest request,
            CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.FromResult(_response);
        }
    }

    private sealed class FakeTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _now;

        public FakeTimeProvider(DateTimeOffset now)
        {
            _now = now;
        }

        public override DateTimeOffset GetUtcNow()
        {
            return _now.ToUniversalTime();
        }

        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.CreateCustomTimeZone(
            "Fake",
            _now.Offset,
            "Fake",
            "Fake");

    }
}
