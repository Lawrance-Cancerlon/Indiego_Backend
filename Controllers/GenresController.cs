using FluentValidation;
using Indiego_Backend.Contracts;
using Indiego_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Indiego_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GenresController(
    IGenreService genreService, 
    IValidator<CreateGenreContract> createGenreValidator
) : ControllerBase
{
    private readonly IGenreService _genreService = genreService;
    private readonly IValidator<CreateGenreContract> _createGenreValidator = createGenreValidator;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? id)
    {
        return Ok(await _genreService.Get(id));
    }

    [HttpPost]
    [Authorize("Admin")]
    public async Task<IActionResult> Create([FromBody] CreateGenreContract createGenreContract)
    {
        var validationResult = await _createGenreValidator.ValidateAsync(createGenreContract);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
        return Ok(await _genreService.Create(createGenreContract));
    }

    [HttpDelete("{id}")]
    [Authorize("Admin")]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        return Ok(await _genreService.Delete(id));
    }
}
