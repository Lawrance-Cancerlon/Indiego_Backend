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
    IUserService userService,
    IGenreService genreService,
    IValidator<CreateGameContract> createGameValidator,
    IValidator<UpdateGameContract> updateGameValidator
) : ControllerBase
{
    private readonly IGameService _gameService = gameService;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IUserService _userService = userService;
    private readonly IGenreService _genreService = genreService;
    private readonly IValidator<CreateGameContract> _createGameValidator = createGameValidator;
    private readonly IValidator<UpdateGameContract> _updateGameValidator = updateGameValidator;
    private readonly string _gamePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "Games");
    private readonly string _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "Images");

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? id, [FromQuery] string? userId, [FromQuery] string? genreId)
    {
        return Ok(await _gameService.Get(id, userId, genreId));
    }

    [HttpGet("{id}")]
    [Authorize("Subscribed")]
    public async Task<IActionResult> Download([FromHeader(Name = "Authorization")] string token, [FromRoute] string id)
    {
        var gameList = await _gameService.Get(id);
        if (gameList.Count == 0) return NotFound();
        var filePath = Path.Combine(_gamePath, $"{id}.zip");
        if (!System.IO.File.Exists(filePath)) return NotFound();
        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();
        await _gameService.Download(id, tokenArr[1]);
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return File(fileStream, "application/zip", $"{gameList[0].Name}.zip");
    }

    [HttpGet("image/{id}")]
    public async Task<IActionResult> GetImage([FromRoute] string id)
    {
        var gameList = await _gameService.Get(id);
        if (gameList.Count == 0) return NotFound();
        var filePath = Path.Combine(_imagePath, $"{id}.png");
        if (!System.IO.File.Exists(filePath)) return NotFound();
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return File(fileStream, "image/png", $"{gameList[0].Name}.png");
    }

    [HttpPost]
    [Authorize("Developer")]
    public async Task<IActionResult> Create([FromHeader(Name = "Authorization")] string token, [FromBody] CreateGameContract create)
    {
        var validationResult = await _createGameValidator.ValidateAsync(create);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();
        var game = await _gameService.Create(create, tokenArr[1]);
        await _userService.GameCreated(tokenArr[1], game!.Id);
        await _genreService.GameCreated(game.Id, create.GenreIds);
        return Ok(game);
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
        if(!Directory.Exists(_gamePath)) Directory.CreateDirectory(_gamePath);
        var filePath = Path.Combine(_gamePath, $"{id}.zip");
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        return Ok(new { message = "Game uploaded successfully"});
    }

    [HttpPost("image/upload/{id}")]
    [Authorize("Developer")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage([FromHeader(Name = "Authorization")] string token, [FromRoute] string id, IFormFile file)
    {
        var gameList = await _gameService.Get(id);
        if (gameList.Count == 0) return NotFound();
        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();
        if (_authenticationService.GetId(tokenArr[1]) != gameList[0].UserId) return Forbid();
        if(file == null || file.Length == 0 || (!file.FileName.EndsWith(".png") && !file.FileName.EndsWith(".jpg") && !file.FileName.EndsWith(".jpeg"))) return BadRequest("File is not valid");
        if(!Directory.Exists(_imagePath)) Directory.CreateDirectory(_imagePath);
        var filePath = Path.Combine(_imagePath, $"{id}.png");
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        return Ok(new { message = "Image uploaded successfully"});
    }

    [HttpPut("{id}")]
    [Authorize("Developer")]
    public async Task<IActionResult> Update([FromHeader(Name = "Authorization")] string token, [FromRoute] string id, [FromBody] UpdateGameContract update)
    {
        var gameList = await _gameService.Get(id);
        if (gameList.Count == 0) return NotFound();
        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();
        if (_authenticationService.GetId(tokenArr[1]) != gameList[0].UserId) return Forbid();
        var validationResult = await _updateGameValidator.ValidateAsync(update);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
        return Ok(await _gameService.Update(id, update));
    }

    [HttpDelete("{id}")]
    [Authorize("Developer")]
    public async Task<IActionResult> Delete([FromHeader(Name = "Authorization")] string token, [FromRoute] string id)
    {
        var gameList = await _gameService.Get(id);
        if (gameList.Count == 0) return NotFound();
        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();
        if (_authenticationService.GetId(tokenArr[1]) != gameList[0].UserId) return Forbid();
        await _userService.GameDeleted(tokenArr[1], id);
        await _genreService.GameDeleted(id, gameList[0].GenreIds);
        var filePath = Path.Combine(_gamePath, $"{id}.zip");
        if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
        return Ok(await _gameService.Delete(id));
    }
}
