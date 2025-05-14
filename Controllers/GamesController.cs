using FluentValidation;
using Indiego_Backend.Contracts;
using Indiego_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Indiego_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController(
    IGameService gameService,
    IAuthenticationService authenticationService,
    IValidator<CreateGameContract> createGameValidator,
    IValidator<UpdateGameContract> updateGameValidator
) : ControllerBase
{
    private readonly IGameService _gameService = gameService;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IValidator<CreateGameContract> _createGameValidator = createGameValidator;
    private readonly IValidator<UpdateGameContract> _updateGameValidator = updateGameValidator;
    private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage");

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? id, [FromQuery] string? developerId, [FromQuery] string? genreId)
    {
        return Ok(await _gameService.Get(id, developerId, genreId));
    }

    [HttpGet("download/{id}")]
    [Authorize]
    public async Task<IActionResult> Download([FromHeader(Name = "Authorization")] string token, [FromRoute] string id)
    {
        var gameList = await _gameService.Get(id);
        if (gameList.Count == 0) return NotFound();
        var filePath = Path.Combine(_storagePath, $"{id}.zip");
        if (!System.IO.File.Exists(filePath)) return NotFound();
        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();
        await _gameService.Download(id, tokenArr[1]);
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return File(fileStream, "application/zip", $"{gameList[0].Name}.zip");
    }

    [HttpPost]
    [Authorize("Developer")]
    public async Task<IActionResult> Create([FromHeader(Name = "Authorization")] string token, [FromBody] CreateGameContract create)
    {
        var validationResult = await _createGameValidator.ValidateAsync(create);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();
        return Ok(await _gameService.Create(create, tokenArr[1]));
    }

    [HttpPost("upload/{id}")]
    [Authorize("Developer")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromHeader(Name = "Authorization")] string token, [FromRoute] string id, IFormFile file)
    {
        var gameList = await _gameService.Get(id);
        if (gameList.Count == 0) return NotFound();
        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();
        if (_authenticationService.GetId(tokenArr[1]) != gameList[0].UserId) return Forbid();
        if(file == null || file.Length == 0 || !file.FileName.EndsWith(".zip")) return BadRequest("File is not valid");
        if(!Directory.Exists(_storagePath)) Directory.CreateDirectory(_storagePath);
        var filePath = Path.Combine(_storagePath, $"{id}.zip");
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        return Ok(new { message = "Game uploaded successfully"});
    }

    // [HttpPut("{id}")]
    // [Authorize("Developer")]

}
