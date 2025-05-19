using FluentValidation;
using Indiego_Backend.Contracts;
using Indiego_Backend.Repositories;
using Indiego_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Indiego_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubscriptionsController(
    ISubscriptionService subscriptionService,
    ISubscriptionTypeService subscriptionTypeService,
    IUserService userService,
    IAuthenticationService authenticationService,
    IValidator<CreateSubscriptionContract> createSubscriptionValidator,
    IValidator<CreateSubscriptionTypeContract> createSubscriptionTypeValidator,
    IValidator<UpdateSubscriptionTypeContract> updateSubscriptionTypeValidator
) : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService = subscriptionService;
    private readonly ISubscriptionTypeService _subscriptionTypeService = subscriptionTypeService;
    private readonly IUserService _userService = userService;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IValidator<CreateSubscriptionContract> _createSubscriptionValidator = createSubscriptionValidator;
    private readonly IValidator<CreateSubscriptionTypeContract> _createSubscriptionTypeValidator = createSubscriptionTypeValidator;
    private readonly IValidator<UpdateSubscriptionTypeContract> _updateSubscriptionTypeValidator = updateSubscriptionTypeValidator;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? id)
    {
        return Ok(await _subscriptionTypeService.Get(id));
    }

    [HttpGet("me")]
    [Authorize("Subscribed")]
    public async Task<IActionResult> GetMySubscriptions([FromHeader(Name = "Authorization")] string token)
    {
        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();
        var userId = _authenticationService.GetId(tokenArr[1]);
        if (userId == null) return BadRequest();
        return Ok(await _subscriptionService.Get(null, userId));
    }

    [HttpPost("new")]
    [Authorize("AdminWithManageSubscriptions")]
    public async Task<IActionResult> Create([FromBody] CreateSubscriptionTypeContract subscriptionType)
    {
        var validationResult = await _createSubscriptionTypeValidator.ValidateAsync(subscriptionType);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
        return Ok(await _subscriptionTypeService.Create(subscriptionType));
    }

    [HttpPost]
    [Authorize("NotSubscribed")]
    public async Task<IActionResult> Subscribe([FromHeader(Name = "Authorization")] string token, [FromBody] CreateSubscriptionContract subscription)
    {
        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();

        var userId = _authenticationService.GetId(tokenArr[1]);
        if (userId == null) return BadRequest();

        var validationResult = await _createSubscriptionValidator.ValidateAsync(subscription);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
        return Ok(await _subscriptionService.Create(subscription, tokenArr[1], _userService));
    }

    [HttpPut("{id}")]
    [Authorize("AdminWithManageSubscriptions")]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateSubscriptionTypeContract subscriptionType)
    {
        var validationResult = await _updateSubscriptionTypeValidator.ValidateAsync(subscriptionType);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
        return Ok(await _subscriptionTypeService.Update(id, subscriptionType));
    }

    [HttpDelete("{id}")]
    [Authorize("AdminWithManageSubscriptions")]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        return Ok(await _subscriptionTypeService.Delete(id));
    }
}
