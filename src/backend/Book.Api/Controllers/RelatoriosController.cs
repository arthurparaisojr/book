using Book.Application.Contracts.Relatorios;
using Book.Application.Services.Relatorios;
using Microsoft.AspNetCore.Mvc;

namespace Book.Api.Controllers;

[ApiController]
[Route("api/v1/relatorios")]
public sealed class RelatoriosController : ControllerBase
{
    private readonly IRelatorioAppService _relatorioAppService;
    private readonly IRelatorioExportAppService _relatorioExportAppService;

    public RelatoriosController(
        IRelatorioAppService relatorioAppService,
        IRelatorioExportAppService relatorioExportAppService)
    {
        _relatorioAppService = relatorioAppService;
        _relatorioExportAppService = relatorioExportAppService;
    }

    [HttpGet("livros-por-autor")]
    [ProducesResponseType(typeof(IReadOnlyList<RelatorioLivroPorAutorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<RelatorioLivroPorAutorResponse>>> GetLivrosPorAutor(
        [FromQuery] string? autorNome,
        CancellationToken cancellationToken)
    {
        var response = await _relatorioAppService.ListLivrosPorAutorAsync(
            new ListRelatorioLivrosPorAutorRequest
            {
                AutorNome = autorNome
            },
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("livros-por-autor/pdf")]
    [Produces("application/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExportLivrosPorAutorPdf(
        [FromQuery] string? autorNome,
        CancellationToken cancellationToken)
    {
        var response = await _relatorioExportAppService.ExportLivrosPorAutorPdfAsync(
            new ListRelatorioLivrosPorAutorRequest
            {
                AutorNome = autorNome
            },
            cancellationToken);

        return File(response.Conteudo, response.TipoConteudo, response.NomeArquivo);
    }
}
