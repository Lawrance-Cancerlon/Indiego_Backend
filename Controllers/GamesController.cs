using System.IO.Compression;
using FluentValidation;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;
using Indiego_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Indiego_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController(
    IGameService gameService,
    IGenreService genreService,
    IReviewService reviewService,
    ISubscriptionService subscriptionService,
    IUserService userService,
    IAuthenticationService authenticationService,
    IValidator<CreateGameContract> createGameValidator,
    IValidator<UpdateGameContract> updateGameValidator
) : ControllerBase
{
    private readonly IGameService _gameService = gameService;
    private readonly IGenreService _genreService = genreService;
    private readonly IReviewService _reviewService = reviewService;
    private readonly IUserService _userService = userService;
    private readonly ISubscriptionService _subscriptionService = subscriptionService;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IValidator<CreateGameContract> _createGameValidator = createGameValidator;
    private readonly IValidator<UpdateGameContract> _updateGameValidator = updateGameValidator;
    private readonly string _gamePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "Games");
    private readonly string _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "Images");

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? id, [FromQuery] string? userId, [FromQuery] string? genreId)
    {
        return Ok(await _gameService.Get(id, userId, genreId));
    }

    [HttpGet("favorites")]
    [Authorize("CustomerOrDeveloper")]
    public async Task<IActionResult> GetFavorites([FromHeader(Name = "Authorization")] string token)
    {
        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();
        var userId = _authenticationService.GetId(tokenArr[1]);
        if (userId == null) return BadRequest();
        return Ok(await _gameService.GetFavorites(userId, _userService));
    }

    [HttpGet("{id}")]
    [Authorize("Subscribed")]
    public async Task<IActionResult> Download([FromHeader(Name = "Authorization")] string token, [FromRoute] string id)
    {
        var game = (await _gameService.Get(id)).FirstOrDefault();
        if (game == null) return NotFound();

        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();

        var filePath = Path.Combine(_gamePath, $"{id}.zip");
        if (!System.IO.File.Exists(filePath)) return NotFound();
        
        await _subscriptionService.AddDownload(tokenArr[1], id, _userService, _gameService);
        await _gameService.Download(id, tokenArr[1], _userService);
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return File(fileStream, "application/zip", $"{game.Name}.zip");
    }

    [HttpGet("image/{id}")]
    public async Task<IActionResult> GetImage([FromRoute] string id)
    {
        var game = (await _gameService.Get(id)).FirstOrDefault();
        if (game == null) return NotFound();
        
        var folderPath = Path.Combine(_imagePath, id);
        if (!Directory.Exists(folderPath)) return NotFound();
        
        var files = Directory.GetFiles(folderPath, $"{id}_*.png");
        if (files.Length == 0) return NotFound();
        
        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var entry = archive.CreateEntry(fileName);
                
                using var entryStream = entry.Open();
                using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                await fileStream.CopyToAsync(entryStream);
            }
        }
        memoryStream.Position = 0;
        return File(memoryStream.ToArray(), "application/zip", $"{game.Id}_images.zip");
    }

    [HttpPost]
    [Authorize("Developer")]
    public async Task<IActionResult> Create([FromHeader(Name = "Authorization")] string token, [FromBody] CreateGameContract create)
    {
        var validationResult = await _createGameValidator.ValidateAsync(create);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();
        var game = await _gameService.Create(create, tokenArr[1], _genreService, _userService);
        return Ok(game);
    }

    [HttpPost("upload/{id}")]
    [Authorize("Developer")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromHeader(Name = "Authorization")] string token, [FromRoute] string id, IFormFile file)
    {
        var game = (await _gameService.Get(id)).FirstOrDefault();
        if (game == null) return NotFound();

        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();

        if (_authenticationService.GetId(tokenArr[1]) != game.UserId) return Forbid();
        
        if (file == null || file.Length == 0 || !file.FileName.EndsWith(".zip")) return BadRequest("File is not valid");
        
        if (!Directory.Exists(_gamePath)) Directory.CreateDirectory(_gamePath);
        var filePath = Path.Combine(_gamePath, $"{id}.zip");
        using var stream = new FileStream(filePath, FileMode.Create);

        await file.CopyToAsync(stream);
        return Ok(new { message = "Game uploaded successfully"});
    }

    [HttpPost("image/upload/{id}")]
    [Authorize("Developer")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage([FromHeader(Name = "Authorization")] string token, [FromRoute] string id, List<IFormFile> files)
    {
        var game = (await _gameService.Get(id)).FirstOrDefault();
        if (game == null) return NotFound();

        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();
        if (_authenticationService.GetId(tokenArr[1]) != game.UserId) return Forbid();
        
        if (files == null || files.Count == 0) return BadRequest("No file provided");
        var path = Path.Combine(_imagePath, $"{id}");
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        int counter = 0;
        foreach (var file in files)
        {
            if (file == null || file.Length == 0 || (!file.FileName.EndsWith(".png") && !file.FileName.EndsWith(".jpg") && !file.FileName.EndsWith(".jpeg"))) return BadRequest("File is not valid");
            var filePath = Path.Combine(path, $"{id}_{counter}.png");
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            counter++;
        }
        return Ok(new { message = "Image uploaded successfully"});
    }

    [HttpPost("favorite/{id}")]
    [Authorize("CustomerOrDeveloper")]
    public async Task<IActionResult> Favorite([FromHeader(Name = "Authorization")] string token, [FromRoute] string id)
    {
        var game = (await _gameService.Get(id)).FirstOrDefault();
        if (game == null) return NotFound();

        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();

        var userId = _authenticationService.GetId(tokenArr[1]);
        if (userId == null) return BadRequest();

        await _userService.AddFavorite(userId, id);
        return Ok(new { message = "Game added to favorites" });
    }

    [HttpPost("unfavorite/{id}")]
    [Authorize("CustomerOrDeveloper")]
    public async Task<IActionResult> Unfavorite([FromHeader(Name = "Authorization")] string token, [FromRoute] string id)
    {
        var game = (await _gameService.Get(id)).FirstOrDefault();
        if (game == null) return NotFound();

        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();

        var userId = _authenticationService.GetId(tokenArr[1]);
        if (userId == null) return BadRequest();

        await _userService.RemoveFavorite(userId, id);
        return Ok(new { message = "Game removed from favorites" });
    }

    [HttpPut("{id}")]
    [Authorize("DeveloperOrAdminWithManageGames")]
    public async Task<IActionResult> Update([FromHeader(Name = "Authorization")] string token, [FromRoute] string id, [FromBody] UpdateGameContract update)
    {
        var game = (await _gameService.Get(id)).FirstOrDefault();
        if (game == null) return NotFound();

        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();

        if (_authenticationService.GetId(tokenArr[1]) != game.UserId && _authenticationService.GetRole(tokenArr[1]) != "Admin") return Forbid();
        
        var validationResult = await _updateGameValidator.ValidateAsync(update);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
        return Ok(await _gameService.Update(id, update));
    }

    [HttpDelete("{id}")]
    [Authorize("DeveloperOrAdminWithManageGames")]
    public async Task<IActionResult> Delete([FromHeader(Name = "Authorization")] string token, [FromRoute] string id)
    {
        var game = (await _gameService.Get(id)).FirstOrDefault();
        if (game == null) return NotFound();

        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();

        if (_authenticationService.GetId(tokenArr[1]) != game.UserId && _authenticationService.GetRole(tokenArr[1]) != "Admin") return Forbid();

        var filePath = Path.Combine(_gamePath, $"{id}.zip");
        if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
        var folderPath = Path.Combine(_imagePath, $"{id}");
        if (Directory.Exists(folderPath)) Directory.Delete(folderPath, true);
        return Ok(await _gameService.Delete(id, _genreService, _reviewService, _userService));
    }
}
