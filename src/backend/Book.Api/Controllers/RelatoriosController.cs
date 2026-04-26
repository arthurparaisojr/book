using Book.Application.Contracts.Relatorios;
using Book.Application.Services.Relatorios;
using Microsoft.AspNetCore.Mvc;

namespace Book.Api.Controllers;

[ApiController]
[Route("api/v1/relatorios")]
public sealed class RelatoriosController : ControllerBase
{
    private readonly IRelatorioAppService _relatorioAppService;

    public RelatoriosController(IRelatorioAppService relatorioAppService)
    {
        _relatorioAppService = relatorioAppService;
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
}
