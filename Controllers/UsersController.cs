using FluentValidation;
using Indiego_Backend.Contracts.Users;
using Indiego_Backend.Models.Users;
using Indiego_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Indiego_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService user, IValidator<CreateAdminContract> adminValidator, IValidator<CreateCustomerContract> customerValidator, IValidator<CreateDeveloperContract> developerValidator) : ControllerBase
    {
        private readonly IUserService _user = user;
        private readonly IValidator<CreateAdminContract> _adminValidator = adminValidator;
        private readonly IValidator<CreateCustomerContract> _customerValidator = customerValidator;
        private readonly IValidator<CreateDeveloperContract> _developerValidator = developerValidator;

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginContract credential)
        {
            var token = await _user.Login(credential);
            if (token == null) return BadRequest();
            return Ok(token);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? id)
        {
            
            return Ok(await _user.Get<UserContract, User>(id));
        }

        [HttpGet("admin")]
        [Authorize("AdminWithManageAdmins")]
        public async Task<IActionResult> GetAdmins([FromQuery] string? id)
        {
            return Ok(await _user.Get<AdminContract, Admin>(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerContract create)
        {
            var validationResult = await _customerValidator.ValidateAsync(create);
            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
            var entity = await _user.Create<CustomerContract, Customer, CreateCustomerContract>(create);
            if (entity == null) return BadRequest();
            return Ok(entity);
        }

        [HttpPost("developer")]
        [Authorize("Customer")]
        public async Task<IActionResult> CreateDeveloper([FromHeader(Name = "Authorization")] string token, [FromBody] CreateDeveloperContract create)
        {
            var entity = await _user.GetLoggedInUser(token);
            if (entity == null) return Unauthorized();
            var validationResult = await _developerValidator.ValidateAsync(create);
            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
            return Ok(await _user.ConvertCustomerToDeveloper(entity.Id, create));
        }

        [HttpPost("admin")]
        [Authorize("AdminWithManageAdmins")]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminContract create)
        {
            var validationResult = await _adminValidator.ValidateAsync(create);
            if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
            var entity = await _user.Create<AdminContract, Admin, CreateAdminContract>(create);
            if (entity == null) return BadRequest();
            return Ok(entity);
        }

        [HttpPut("{id}")]
        [Authorize("Customer")]
        public async Task<IActionResult> Update([FromBody] UpdateCustomerContract update, string id)
        {
            var entity = await _user.Update<CustomerContract, Customer, UpdateCustomerContract>(id, update);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPut("developer/{id}")]
        [Authorize("Developer")]
        public async Task<IActionResult> UpdateDeveloper([FromBody] UpdateDeveloperContract update, string id)
        {
            var entity = await _user.Update<DeveloperContract, Developer, UpdateDeveloperContract>(id, update);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPut("admin/{id}")]
        [Authorize("AdminWithManageAdmins")]
        public async Task<IActionResult> UpdateAdmin([FromBody] UpdateAdminContract update, string id)
        {
            var entity = await _user.Update<AdminContract, Admin, UpdateAdminContract>(id, update);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete([FromHeader(Name = "Authorization")] string token)
        {
            var entity = await _user.GetLoggedInUser(token);
            if (entity == null) return Unauthorized();
            return Ok(await _user.Delete<UserContract>(entity.Id));
        }

        [HttpDelete("admin/{id}")]
        [Authorize("AdminWithManageAdmins")]
        public async Task<IActionResult> DeleteAdmin(string id)
        {
            var entity = await _user.Delete<AdminContract>(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }
    }
}
