using Book.Application.Contracts.Livros;
using Book.Application.Services.Livros;
using Book.Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book.Api.Controllers;

[ApiController]
[Route("api/v1/livros")]
public sealed class LivrosController : ControllerBase
{
    private readonly ILivroAppService _livroAppService;

    public LivrosController(ILivroAppService livroAppService)
    {
        _livroAppService = livroAppService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<LivroResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LivroResponse>>> GetAll(
        [FromQuery] string? titulo,
        [FromQuery] string? autorNome,
        [FromQuery] string? assuntoDescricao,
        CancellationToken cancellationToken)
    {
        var response = await _livroAppService.ListAsync(
            new ListLivrosRequest
            {
                Titulo = titulo,
                AutorNome = autorNome,
                AssuntoDescricao = assuntoDescricao
            },
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("{codl:int}")]
    [ProducesResponseType(typeof(LivroResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LivroResponse>> GetById(int codl, CancellationToken cancellationToken)
    {
        var response = await _livroAppService.GetByIdAsync(codl, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Policy = CatalogPolicies.CatalogWrite)]
    [ProducesResponseType(typeof(LivroResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LivroResponse>> Create(
        [FromBody] CreateLivroRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _livroAppService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { codl = response.Codl }, response);
    }

    [HttpPut("{codl:int}")]
    [Authorize(Policy = CatalogPolicies.CatalogWrite)]
    [ProducesResponseType(typeof(LivroResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LivroResponse>> Update(
        int codl,
        [FromBody] UpdateLivroRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _livroAppService.UpdateAsync(codl, request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{codl:int}")]
    [Authorize(Policy = CatalogPolicies.CatalogWrite)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int codl, CancellationToken cancellationToken)
    {
        await _livroAppService.DeleteAsync(codl, cancellationToken);
        return NoContent();
    }
}
