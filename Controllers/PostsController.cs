using FluentValidation;
using Indiego_Backend.Contracts;
using Indiego_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Indiego_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostsController(
    IPostService postService,
    IUserService userService,
    IAuthenticationService authenticationService,
    IValidator<CreatePostContract> createPostValidator,
    IValidator<UpdatePostContract> updatePostValidator
) : ControllerBase
{
    private readonly IPostService _postService = postService;
    private readonly IUserService _userService = userService;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IValidator<CreatePostContract> _createPostValidator = createPostValidator;
    private readonly IValidator<UpdatePostContract> _updatePostValidator = updatePostValidator;
    private readonly string _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "Images");


    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? id, [FromQuery] string? userId)
    {
        return Ok(await _postService.Get(id, userId));
    }

    [HttpGet("like")]
    [Authorize("CustomerOrDeveloper")]
    public async Task<IActionResult> GetLikes([FromHeader(Name = "Authorization")] string token)
    {
        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();
        var userId = _authenticationService.GetId(tokenArr[1]);
        if (userId == null) return BadRequest();
        return Ok(await _postService.GetLikes(userId, _userService));
    }

    [HttpPost]
    [Authorize("Developer")]
    public async Task<IActionResult> Create([FromHeader(Name = "Authorization")] string token, [FromBody] CreatePostContract create)
    {
        var validationResult = await _createPostValidator.ValidateAsync(create);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();

        var post = await _postService.Create(create, tokenArr[1], _userService);
        return Ok(post);
    }

    [HttpPost("upload/{id}")]
    [Authorize("Developer")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromHeader(Name = "Authorization")] string token, [FromRoute] string id, IFormFile file)
    {
        var post = (await _postService.Get(id)).FirstOrDefault();
        if (post == null) return NotFound();

        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();

        if (_authenticationService.GetId(tokenArr[1]) != post.UserId) return Forbid();

        if (file == null || file.Length == 0 || (!file.FileName.EndsWith(".png") && !file.FileName.EndsWith(".jpg") && !file.FileName.EndsWith(".jpeg"))) return BadRequest("File is not valid");
        if (!Directory.Exists(_imagePath)) Directory.CreateDirectory(_imagePath);
        var filePath = Path.Combine(_imagePath, $"{id}.png");
        using var stream = new FileStream(filePath, FileMode.Create);

        await file.CopyToAsync(stream);
        return Ok(new { message = "Image uploaded successfully" });
    }

    [HttpPost("like/{id}")]
    [Authorize("CustomerOrDeveloper")]
    public async Task<IActionResult> Like([FromHeader(Name = "Authorization")] string token, [FromRoute] string id)
    {
        var post = (await _postService.Get(id)).FirstOrDefault();
        if (post == null) return NotFound();

        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();

        await _postService.AddLike(id, tokenArr[1], _userService);
        return Ok(new { message = "Post added to likes" });
    }

    [HttpPost("unlike/{id}")]
    [Authorize("CustomerOrDeveloper")]
    public async Task<IActionResult> Unlike([FromHeader(Name = "Authorization")] string token, [FromRoute] string id)
    {
        var post = (await _postService.Get(id)).FirstOrDefault();
        if (post == null) return NotFound();

        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();

        await _postService.RemoveLike(id, tokenArr[1], _userService);
        return Ok(new { message = "Post removed from likes" });
    }

    [HttpPut("{id}")]
    [Authorize("DeveloperOrAdminWithManagePosts")]
    public async Task<IActionResult> Update([FromHeader(Name = "Authorization")] string token, [FromRoute] string id, [FromBody] UpdatePostContract update)
    {
        var post = (await _postService.Get(id)).FirstOrDefault();
        if (post == null) return NotFound();

        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();

        if (_authenticationService.GetId(tokenArr[1]) != post.UserId && _authenticationService.GetRole(tokenArr[1]) != "Admin") return Forbid();

        var validationResult = await _updatePostValidator.ValidateAsync(update);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
        return Ok(await _postService.Update(id, update));
    }

    [HttpDelete("{id}")]
    [Authorize("DeveloperOrAdminWithManagePosts")]
    public async Task<IActionResult> Delete([FromHeader(Name = "Authorization")] string token, [FromRoute] string id)
    {
        var post = (await _postService.Get(id)).FirstOrDefault();
        if (post == null) return NotFound();

        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();

        if (_authenticationService.GetId(tokenArr[1]) != post.UserId && _authenticationService.GetRole(tokenArr[1]) != "Admin") return Forbid();

        var filePath = Path.Combine(_imagePath, $"{id}.png");
        if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
        return Ok(await _postService.Delete(id, _userService));
    }
}
