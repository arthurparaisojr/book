using Book.Application.Contracts.Autores;
using Book.Application.Services.Autores;
using Book.Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book.Api.Controllers;

[ApiController]
[Route("api/v1/autores")]
public sealed class AutoresController : ControllerBase
{
    private readonly IAutorAppService _autorAppService;

    public AutoresController(IAutorAppService autorAppService)
    {
        _autorAppService = autorAppService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AutorResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AutorResponse>>> GetAll(
        [FromQuery] string? nome,
        CancellationToken cancellationToken)
    {
        var response = await _autorAppService.ListAsync(
            new ListAutoresRequest
            {
                Nome = nome
            },
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("{codAu:int}")]
    [ProducesResponseType(typeof(AutorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AutorResponse>> GetById(int codAu, CancellationToken cancellationToken)
    {
        var response = await _autorAppService.GetByIdAsync(codAu, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Policy = CatalogPolicies.CatalogWrite)]
    [ProducesResponseType(typeof(AutorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<AutorResponse>> Create(
        [FromBody] CreateAutorRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _autorAppService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { codAu = response.CodAu }, response);
    }

    [HttpPut("{codAu:int}")]
    [Authorize(Policy = CatalogPolicies.CatalogWrite)]
    [ProducesResponseType(typeof(AutorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<AutorResponse>> Update(
        int codAu,
        [FromBody] UpdateAutorRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _autorAppService.UpdateAsync(codAu, request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{codAu:int}")]
    [Authorize(Policy = CatalogPolicies.CatalogWrite)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int codAu, CancellationToken cancellationToken)
    {
        await _autorAppService.DeleteAsync(codAu, cancellationToken);
        return NoContent();
    }
}
