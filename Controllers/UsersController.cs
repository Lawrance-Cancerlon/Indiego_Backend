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
public class UsersController(
    IUserService userService,
    IAuthenticationService authenticationService,
    IValidator<CreateAdminContract> createAdminValidator,
    IValidator<UpdateAdminContract> updateAdminValidator,
    IValidator<CreateCustomerContract> createCustomerValidator,
    IValidator<UpdateCustomerContract> updateCustomerValidator,
    IValidator<CreateDeveloperContract> createDeveloperValidator,
    IValidator<UpdateDeveloperContract> updateDeveloperValidator
) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IValidator<CreateAdminContract> _createAdminValidator = createAdminValidator;
    private readonly IValidator<UpdateAdminContract> _updateAdminValidator = updateAdminValidator;
    private readonly IValidator<CreateCustomerContract> _createCustomerValidator = createCustomerValidator;
    private readonly IValidator<UpdateCustomerContract> _updateCustomerValidator = updateCustomerValidator;
    private readonly IValidator<CreateDeveloperContract> _createDeveloperValidator = createDeveloperValidator;
    private readonly IValidator<UpdateDeveloperContract> _updateDeveloperValidator = updateDeveloperValidator;

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe([FromHeader(Name = "Authorization")] string token)
    {
        var tokenArr = token.Split(" ");
        if(tokenArr.Length != 2) return BadRequest();
        var role = _authenticationService.GetRole(tokenArr[1]);
        if (role == "Admin") 
            return Ok(await _userService.GetLoggedInUser<AdminContract, Admin>(tokenArr[1]));
        else if (role == "Developer")
            return Ok(await _userService.GetLoggedInUser<DeveloperContract, Developer>(tokenArr[1]));
        else if (role == "Customer")
            return Ok(await _userService.GetLoggedInUser<CustomerContract, Customer>(tokenArr[1]));
        else
            return Ok(await _userService.GetLoggedInUser<UserContract, User>(tokenArr[1]));
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? id)
    {
        return Ok(await _userService.Get<UserContract, User>(id));
    }

    [HttpGet("developer")]
    public async Task<IActionResult> GetDeveloper([FromQuery] string? id)
    {
        return Ok(await _userService.Get<DeveloperContract, Developer>(id));
    }

    [HttpGet("admin")]
    [Authorize("AdminWithManageAdmins")]
    public async Task<IActionResult> GetAdmin([FromQuery] string? id)
    {
        return Ok(await _userService.Get<AdminContract, Admin>(id));
    }

    [HttpGet("download")]
    [Authorize("Developer")]
    public async Task<IActionResult> GetDownload([FromHeader(Name = "Authorization")] string token)
    {
        var tokenArr = token.Split(" ");
        if(tokenArr.Length != 2) return BadRequest();
        var userId = _authenticationService.GetId(tokenArr[1]);
        if(userId == null) return BadRequest();
        var user = (await _userService.Get<DeveloperContract, Developer>(userId)).FirstOrDefault();
        if (user == null) return BadRequest();
        return Ok(await _userService.GetDownloadAnalytics(userId));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginContract loginContract)
    {
        var token = await _userService.Login(loginContract);
        if (token == null) return BadRequest();
        return Ok(token);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerContract createCustomerContract)
    {
        var validationResult = await _createCustomerValidator.ValidateAsync(createCustomerContract);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
        return Ok(await _userService.Create<CustomerContract, Customer, CreateCustomerContract>(createCustomerContract));
    }

    [HttpPost("developer")]
    [Authorize("Customer")]
    public async Task<IActionResult> CreateDeveloper([FromHeader(Name = "Authorization")] string token, [FromBody] CreateDeveloperContract createDeveloperContract)
    {
        var validationResult = await _createDeveloperValidator.ValidateAsync(createDeveloperContract);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
        var tokenArr = token.Split(" ");
        if(tokenArr.Length != 2) return BadRequest();
        return Ok(await _userService.ConvertCustomerToDeveloper(tokenArr[1], createDeveloperContract));
    }

    [HttpPost("admin")]
    [Authorize("AdminWithManageAdmins")]
    public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminContract createAdminContract)
    {
        var validationResult = await _createAdminValidator.ValidateAsync(createAdminContract);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
        return Ok(await _userService.Create<AdminContract, Admin, CreateAdminContract>(createAdminContract));
    }

    [HttpPost("withdraw/{amount}")]
    [Authorize("Developer")]
    public async Task<IActionResult> Withdraw([FromHeader(Name = "Authorization")] string token, [FromRoute] int amount)
    {
        var tokenArr = token.Split(" ");
        if (tokenArr.Length != 2) return BadRequest();

        var userId = _authenticationService.GetId(tokenArr[1]);
        if (userId == null) return BadRequest();

        var user = (await _userService.Get<DeveloperContract, Developer>(userId)).FirstOrDefault();
        if (user == null) return BadRequest();
        if (user.Balance < amount) return BadRequest("Not enough balance");
        
        await _userService.RemoveBalance(userId, amount);
        return Ok(new { message = "Withdraw balance successful" });
    }

    [HttpPut]
    [Authorize("CustomerOrDeveloper")]
    public async Task<IActionResult> Update([FromHeader(Name = "Authorization")] string token, [FromBody] UpdateCustomerContract updateCustomerContract)
    {
        var validationResult = await _updateCustomerValidator.ValidateAsync(updateCustomerContract);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
        var tokenArr = token.Split(" ");
        if(tokenArr.Length != 2) return BadRequest();
        var userId = _authenticationService.GetId(tokenArr[1]);
        if(userId == null) return BadRequest();
        return Ok(await _userService.Update<CustomerContract, Customer, UpdateCustomerContract>(userId, updateCustomerContract));
    }

    [HttpPut("developer")]
    [Authorize("Developer")]
    public async Task<IActionResult> UpdateDeveloper([FromHeader(Name = "Authorization")] string token, [FromBody] UpdateDeveloperContract updateDeveloperContract)
    {
        var validationResult = await _updateDeveloperValidator.ValidateAsync(updateDeveloperContract);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
        var tokenArr = token.Split(" ");
        if(tokenArr.Length != 2) return BadRequest();
        var userId = _authenticationService.GetId(tokenArr[1]);
        if(userId == null) return BadRequest();
        return Ok(await _userService.Update<DeveloperContract, Developer, UpdateDeveloperContract>(userId, updateDeveloperContract));
    }

    [HttpPut("admin/{id}")]
    [Authorize("AdminWithManageAdmins")]
    public async Task<IActionResult> UpdateAdmin([FromRoute] string id, [FromBody] UpdateAdminContract updateAdminContract)
    {
        var validationResult = await _updateAdminValidator.ValidateAsync(updateAdminContract);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
        return Ok(await _userService.Update<AdminContract, Admin, UpdateAdminContract>(id, updateAdminContract));
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> Delete([FromHeader(Name = "Authorization")] string token)
    {
        var tokenArr = token.Split(" ");
        if(tokenArr.Length != 2) return BadRequest();
        var userId = _authenticationService.GetId(tokenArr[1]);
        if(userId == null) return BadRequest();
        var role = _authenticationService.GetRole(tokenArr[1]);
        if (role == "Admin") 
            return Ok(await _userService.Delete<AdminContract, Admin>(userId));
        else if (role == "Developer")
            return Ok(await _userService.Delete<DeveloperContract, Developer>(userId));
        else if (role == "Customer")
            return Ok(await _userService.Delete<CustomerContract, Customer>(userId));
        else
            return Ok(await _userService.Delete<UserContract, User>(userId));
    }

    [HttpDelete("admin/{id}")]
    [Authorize("AdminWithManageAdmins")]
    public async Task<IActionResult> DeleteAdmin([FromRoute] string id)
    {
        return Ok(await _userService.Delete<AdminContract, Admin>(id));
    }
}