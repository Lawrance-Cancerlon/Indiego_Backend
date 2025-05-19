using FluentValidation;
using Indiego_Backend.Contracts;
using Indiego_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Indiego_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController(
    IGameService gameService,
    IReviewService reviewService,
    IUserService userService,
    IAuthenticationService authenticationService,
    IValidator<CreateReviewContract> createReviewValidator,
    IValidator<UpdateReviewContract> updateReviewValidator
) : ControllerBase
{
    private readonly IGameService _gameService = gameService;
    private readonly IReviewService _reviewService = reviewService;
    private readonly IUserService _userService = userService;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IValidator<CreateReviewContract> _createReviewValidator = createReviewValidator;
    private readonly IValidator<UpdateReviewContract> _updateReviewValidator = updateReviewValidator;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? id, [FromQuery] string? gameId, [FromQuery] string? userId)
    {
        return Ok(await _reviewService.Get(id, gameId, userId));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromHeader(Name = "Authorization")] string token, [FromBody] CreateReviewContract create)
    {
        var validationResult = await _createReviewValidator.ValidateAsync(create);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();
        var review = await _reviewService.Create(create, tokenArr[1], _userService, _gameService);
        return Ok(review);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update([FromHeader(Name = "Authorization")] string token, [FromRoute] string id, [FromBody] UpdateReviewContract update)
    {
        var review = (await _reviewService.Get(id)).FirstOrDefault();
        if (review == null) return NotFound();

        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();

        if (_authenticationService.GetId(tokenArr[1]) != review.UserId) return Unauthorized();

        var validationResult = await _updateReviewValidator.ValidateAsync(update);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
        return Ok(await _reviewService.Update(id, update));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete([FromHeader(Name = "Authorization")] string token, [FromRoute] string id)
    {
        var review = (await _reviewService.Get(id)).FirstOrDefault();
        if (review == null) return NotFound();

        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();

        if (_authenticationService.GetId(tokenArr[1]) != review.UserId) return Unauthorized();

        return Ok(await _reviewService.Delete(id, _userService, _gameService));
    }
}
