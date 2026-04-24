using Book.Application.Contracts.Assuntos;
using Book.Application.Services.Assuntos;
using Book.Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book.Api.Controllers;

[ApiController]
[Route("api/v1/assuntos")]
public sealed class AssuntosController : ControllerBase
{
    private readonly IAssuntoAppService _assuntoAppService;

    public AssuntosController(IAssuntoAppService assuntoAppService)
    {
        _assuntoAppService = assuntoAppService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AssuntoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AssuntoResponse>>> GetAll(
        [FromQuery] string? descricao,
        CancellationToken cancellationToken)
    {
        var response = await _assuntoAppService.ListAsync(
            new ListAssuntosRequest
            {
                Descricao = descricao
            },
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("{codAs:int}")]
    [ProducesResponseType(typeof(AssuntoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssuntoResponse>> GetById(int codAs, CancellationToken cancellationToken)
    {
        var response = await _assuntoAppService.GetByIdAsync(codAs, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Policy = CatalogPolicies.CatalogWrite)]
    [ProducesResponseType(typeof(AssuntoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<AssuntoResponse>> Create(
        [FromBody] CreateAssuntoRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _assuntoAppService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { codAs = response.CodAs }, response);
    }

    [HttpPut("{codAs:int}")]
    [Authorize(Policy = CatalogPolicies.CatalogWrite)]
    [ProducesResponseType(typeof(AssuntoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<AssuntoResponse>> Update(
        int codAs,
        [FromBody] UpdateAssuntoRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _assuntoAppService.UpdateAsync(codAs, request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{codAs:int}")]
    [Authorize(Policy = CatalogPolicies.CatalogWrite)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int codAs, CancellationToken cancellationToken)
    {
        await _assuntoAppService.DeleteAsync(codAs, cancellationToken);
        return NoContent();
    }
}
