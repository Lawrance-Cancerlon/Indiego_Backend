using FluentValidation;
using FluentValidation.Results;
using Indiego_Backend.Contracts;
using Indiego_Backend.Controllers;
using Indiego_Backend.Models;
using Indiego_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Indiego_Backend.Tests
{
    public class UserControllerTest
    {
        private UsersController _controller = null!;
        private Mock<IUserService> _mockUserService = null!;
        private Mock<IAuthenticationService> _mockAuthService = null!;
        private Mock<IValidator<CreateAdminContract>> _mockCreateAdminValidator = null!;
        private Mock<IValidator<UpdateAdminContract>> _mockUpdateAdminValidator = null!;
        private Mock<IValidator<CreateCustomerContract>> _mockCreateCustomerValidator = null!;
        private Mock<IValidator<UpdateCustomerContract>> _mockUpdateCustomerValidator = null!;

        [SetUp]
        public void Setup()
        {
            _mockUserService = new Mock<IUserService>();
            _mockAuthService = new Mock<IAuthenticationService>();
            _mockCreateAdminValidator = new Mock<IValidator<CreateAdminContract>>();
            _mockUpdateAdminValidator = new Mock<IValidator<UpdateAdminContract>>();
            _mockCreateCustomerValidator = new Mock<IValidator<CreateCustomerContract>>();
            _mockUpdateCustomerValidator = new Mock<IValidator<UpdateCustomerContract>>();

            _controller = new UsersController(
                _mockUserService.Object,
                _mockAuthService.Object,
                _mockCreateAdminValidator.Object,
                _mockUpdateAdminValidator.Object,
                _mockCreateCustomerValidator.Object,
                _mockUpdateCustomerValidator.Object
            );
        }

        [Test]
        public async Task GetMe_WithAdminRole_ReturnsAdmin()
        {
            // Arrange
            var token = "Bearer testToken";
            var adminContract = new AdminContract();
            _mockAuthService.Setup(s => s.GetRole("testToken")).Returns("Admin");
            _mockUserService.Setup(s => s.GetLoggedInUser<AdminContract, Admin>("testToken"))
                .ReturnsAsync(adminContract);

            // Act
            var result = await _controller.GetMe(token) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(adminContract));
        }

        [Test]
        public async Task GetMe_WithDeveloperRole_ReturnsDeveloper()
        {
            // Arrange
            var token = "Bearer testToken";
            var developerContract = new DeveloperContract();
            _mockAuthService.Setup(s => s.GetRole("testToken")).Returns("Developer");
            _mockUserService.Setup(s => s.GetLoggedInUser<DeveloperContract, Developer>("testToken"))
                .ReturnsAsync(developerContract);

            // Act
            var result = await _controller.GetMe(token) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(developerContract));
        }

        [Test]
        public async Task GetMe_WithCustomerRole_ReturnsCustomer()
        {
            // Arrange
            var token = "Bearer testToken";
            var customerContract = new CustomerContract();
            _mockAuthService.Setup(s => s.GetRole("testToken")).Returns("Customer");
            _mockUserService.Setup(s => s.GetLoggedInUser<CustomerContract, Customer>("testToken"))
                .ReturnsAsync(customerContract);

            // Act
            var result = await _controller.GetMe(token) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(customerContract));
        }

        [Test]
        public async Task GetMe_WithUnknownRole_ReturnsUser()
        {
            // Arrange
            var token = "Bearer testToken";
            var userContract = new UserContract();
            _mockAuthService.Setup(s => s.GetRole("testToken")).Returns("Unknown");
            _mockUserService.Setup(s => s.GetLoggedInUser<UserContract, User>("testToken"))
                .ReturnsAsync(userContract);

            // Act
            var result = await _controller.GetMe(token) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(userContract));
        }

        [Test]
        public async Task GetMe_WithInvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var token = "InvalidToken";

            // Act
            var result = await _controller.GetMe(token);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task Get_ReturnsOk()
        {
            // Arrange
            var userContract = new UserContract();
            var userContractsList = new List<UserContract> { userContract };
            _mockUserService.Setup(s => s.Get<UserContract, User>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(userContractsList);

            // Act
            var result = await _controller.Get("1") as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(userContractsList));
        }

        [Test]
        public async Task Get_ReturnsEmptyArray_WhenNoUsersFound()
        {
            // Arrange
            var emptyList = new List<UserContract>();
            _mockUserService.Setup(s => s.Get<UserContract, User>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _controller.Get("1") as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(emptyList));
            Assert.That((result.Value as List<UserContract>)?.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetDeveloper_ReturnsOk()
        {
            // Arrange
            var developerContract = new DeveloperContract();
            var developerContractsList = new List<DeveloperContract> { developerContract };
            _mockUserService.Setup(s => s.Get<DeveloperContract, Developer>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(developerContractsList);

            // Act
            var result = await _controller.GetDeveloper("1") as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(developerContractsList));
        }

        [Test]
        public async Task GetDeveloper_ReturnsEmptyArray_WhenNoUsersFound()
        {
            // Arrange
            var emptyList = new List<DeveloperContract>();
            _mockUserService.Setup(s => s.Get<DeveloperContract, Developer>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _controller.GetDeveloper("1") as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(emptyList));
            Assert.That((result.Value as List<DeveloperContract>)?.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAdmin_ReturnsOk()
        {
            // Arrange
            var adminContract = new AdminContract();
            var adminContractsList = new List<AdminContract> { adminContract };
            _mockUserService.Setup(s => s.Get<AdminContract, Admin>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(adminContractsList);

            // Act
            var result = await _controller.GetAdmin("1") as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(adminContractsList));
        }

        [Test]
        public async Task GetAdmin_ReturnsEmptyArray_WhenNoAdminsFound()
        {
            // Arrange
            var emptyList = new List<AdminContract>();
            _mockUserService.Setup(s => s.Get<AdminContract, Admin>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _controller.GetAdmin("1") as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(emptyList));
            Assert.That((result.Value as List<AdminContract>)?.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginContract = new LoginContract { Email = "test@test.com", Password = "password" };
            var token = "testToken";
            var loginResponse = new LoginResponseContract { Token = token };
            _mockUserService.Setup(s => s.Login(loginContract)).ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.Login(loginContract) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(loginResponse));
        }
        [Test]
        public async Task Login_WithInvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var loginContract = new LoginContract { Email = "test@test.com", Password = "wrongpassword" };
            _mockUserService.Setup(s => s.Login(loginContract)).ReturnsAsync(() => null!);

            // Act
            var result = await _controller.Login(loginContract);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task Create_WithValidData_ReturnsOk()
        {
            // Arrange
            var createCustomerContract = new CreateCustomerContract();
            var customerContract = new CustomerContract();
            
            var validationResult = new ValidationResult();
            _mockCreateCustomerValidator.Setup(v => v.ValidateAsync(createCustomerContract, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            _mockUserService.Setup(s => s.Create<CustomerContract, Customer, CreateCustomerContract>(createCustomerContract))
                .ReturnsAsync(customerContract);

            // Act
            var result = await _controller.Create(createCustomerContract) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(customerContract));
        }

        [Test]
        public async Task Create_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var createCustomerContract = new CreateCustomerContract();
            var validationFailures = new List<ValidationFailure>
            {
                new("Email", "Email is required")
            };
            
            var validationResult = new ValidationResult(validationFailures);
            _mockCreateCustomerValidator.Setup(v => v.ValidateAsync(createCustomerContract, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.Create(createCustomerContract) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo(validationFailures));
        }

        [Test]
        public async Task CreateDeveloper_ReturnsOk()
        {
            // Arrange
            var token = "Bearer testToken";
            var developerContract = new DeveloperContract();
            _mockUserService.Setup(s => s.ConvertCustomerToDeveloper("testToken"))
                .ReturnsAsync(developerContract);

            // Act
            var result = await _controller.CreateDeveloper(token) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(developerContract));
        }

        [Test]
        public async Task CreateDeveloper_WithInvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var token = "InvalidToken";

            // Act
            var result = await _controller.CreateDeveloper(token);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task CreateAdmin_WithValidData_ReturnsOk()
        {
            // Arrange
            var createAdminContract = new CreateAdminContract();
            var adminContract = new AdminContract();
            
            var validationResult = new ValidationResult();
            _mockCreateAdminValidator.Setup(v => v.ValidateAsync(createAdminContract, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            _mockUserService.Setup(s => s.Create<AdminContract, Admin, CreateAdminContract>(createAdminContract))
                .ReturnsAsync(adminContract);

            // Act
            var result = await _controller.CreateAdmin(createAdminContract) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(adminContract));
        }

        [Test]
        public async Task CreateAdmin_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var createAdminContract = new CreateAdminContract();
            var validationFailures = new List<ValidationFailure>
            {
                new("Email", "Email is required")
            };
            
            var validationResult = new ValidationResult(validationFailures);
            _mockCreateAdminValidator.Setup(v => v.ValidateAsync(createAdminContract, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.CreateAdmin(createAdminContract) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo(validationFailures));
        }

        [Test]
        public async Task Withdraw_WithValidTokenAndSufficientBalance_ReturnsOk()
        {
            // Arrange
            var token = "Bearer testToken";
            var userId = "userId";
            var amount = 100;
            var developerContract = new DeveloperContract { Balance = 500 };
            var developerContractsList = new List<DeveloperContract> { developerContract };

            _mockAuthService.Setup(s => s.GetId("testToken")).Returns(userId);
            _mockUserService.Setup(s => s.Get<DeveloperContract, Developer>(userId, null))
                .ReturnsAsync(developerContractsList);
            _mockUserService.Setup(s => s.RemoveBalance(userId, amount)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Withdraw(token, amount) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That((result.Value as dynamic)?.message, Is.EqualTo("Withdraw balance successful"));
            _mockUserService.Verify(s => s.RemoveBalance(userId, amount), Times.Once);
        }

        [Test]
        public async Task Withdraw_WithInvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var token = "InvalidToken";
            var amount = 100;

            // Act
            var result = await _controller.Withdraw(token, amount);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
            _mockUserService.Verify(s => s.RemoveBalance(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task Withdraw_WithNullUserId_ReturnsBadRequest()
        {
            // Arrange
            var token = "Bearer testToken";
            var amount = 100;

            _mockAuthService.Setup(s => s.GetId("testToken")).Returns((string?)null);

            // Act
            var result = await _controller.Withdraw(token, amount);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
            _mockUserService.Verify(s => s.RemoveBalance(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task Withdraw_WithNonExistentUser_ReturnsBadRequest()
        {
            // Arrange
            var token = "Bearer testToken";
            var userId = "userId";
            var amount = 100;
            var emptyList = new List<DeveloperContract>();

            _mockAuthService.Setup(s => s.GetId("testToken")).Returns(userId);
            _mockUserService.Setup(s => s.Get<DeveloperContract, Developer>(userId, null))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _controller.Withdraw(token, amount);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
            _mockUserService.Verify(s => s.RemoveBalance(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task Withdraw_WithInsufficientBalance_ReturnsBadRequestWithMessage()
        {
            // Arrange
            var token = "Bearer testToken";
            var userId = "userId";
            var amount = 200;
            var developerContract = new DeveloperContract { Balance = 100 };
            var developerContractsList = new List<DeveloperContract> { developerContract };

            _mockAuthService.Setup(s => s.GetId("testToken")).Returns(userId);
            _mockUserService.Setup(s => s.Get<DeveloperContract, Developer>(userId, null))
                .ReturnsAsync(developerContractsList);

            // Act
            var result = await _controller.Withdraw(token, amount) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo("Not enough balance"));
            _mockUserService.Verify(s => s.RemoveBalance(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task Update_WithValidData_ReturnsOk()
        {
            // Arrange
            var token = "Bearer testToken";
            var updateCustomerContract = new UpdateCustomerContract();
            var customerContract = new CustomerContract();
            
            var validationResult = new ValidationResult();
            _mockUpdateCustomerValidator.Setup(v => v.ValidateAsync(updateCustomerContract, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            _mockAuthService.Setup(s => s.GetId("testToken")).Returns("userId");
            _mockUserService.Setup(s => s.Update<CustomerContract, Customer, UpdateCustomerContract>("userId", updateCustomerContract))
                .ReturnsAsync(customerContract);

            // Act
            var result = await _controller.Update(token, updateCustomerContract) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(customerContract));
        }

        [Test]
        public async Task Update_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var token = "Bearer testToken";
            var updateCustomerContract = new UpdateCustomerContract();
            var validationFailures = new List<ValidationFailure>
            {
                new("Email", "Email is required")
            };
            
            var validationResult = new ValidationResult(validationFailures);
            _mockUpdateCustomerValidator.Setup(v => v.ValidateAsync(updateCustomerContract, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.Update(token, updateCustomerContract) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo(validationFailures));
        }

        [Test]
        public async Task Update_WithInvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var token = "InvalidToken";
            var updateCustomerContract = new UpdateCustomerContract();
            var validationResult = new ValidationResult();
            
            _mockUpdateCustomerValidator.Setup(v => v.ValidateAsync(updateCustomerContract, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.Update(token, updateCustomerContract);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task Update_WithNullUserId_ReturnsBadRequest()
        {
            // Arrange
            var token = "Bearer testToken";
            var updateCustomerContract = new UpdateCustomerContract();
            var validationResult = new ValidationResult();
            
            _mockUpdateCustomerValidator.Setup(v => v.ValidateAsync(updateCustomerContract, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
            
            _mockAuthService.Setup(s => s.GetId("testToken")).Returns((string?)null);

            // Act
            var result = await _controller.Update(token, updateCustomerContract);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task UpdateAdmin_WithValidData_ReturnsOk()
        {
            // Arrange
            var updateAdminContract = new UpdateAdminContract();
            var adminContract = new AdminContract();
            
            var validationResult = new ValidationResult();
            _mockUpdateAdminValidator.Setup(v => v.ValidateAsync(updateAdminContract, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            _mockUserService.Setup(s => s.Update<AdminContract, Admin, UpdateAdminContract>("adminId", updateAdminContract))
                .ReturnsAsync(adminContract);

            // Act
            var result = await _controller.UpdateAdmin("adminId", updateAdminContract) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(adminContract));
        }

        [Test]
        public async Task UpdateAdmin_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var updateAdminContract = new UpdateAdminContract();
            var validationFailures = new List<ValidationFailure>
            {
                new("Name", "Name is required")
            };
            
            var validationResult = new ValidationResult(validationFailures);
            _mockUpdateAdminValidator.Setup(v => v.ValidateAsync(updateAdminContract, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.UpdateAdmin("adminId", updateAdminContract) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo(validationFailures));
        }

        [Test]
        public async Task Delete_WithAdminRole_ReturnsOk()
        {
            // Arrange
            var token = "Bearer testToken";
            var adminContract = new AdminContract();

            _mockAuthService.Setup(s => s.GetId("testToken")).Returns("userId");
            _mockAuthService.Setup(s => s.GetRole("testToken")).Returns("Admin");
            _mockUserService.Setup(s => s.Delete<AdminContract, Admin>("userId"))
                .ReturnsAsync(adminContract);

            // Act
            var result = await _controller.Delete(token) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(adminContract));
        }

        [Test]
        public async Task Delete_WithDeveloperRole_ReturnsOk()
        {
            // Arrange
            var token = "Bearer testToken";
            var developerContract = new DeveloperContract();

            _mockAuthService.Setup(s => s.GetId("testToken")).Returns("userId");
            _mockAuthService.Setup(s => s.GetRole("testToken")).Returns("Developer");
            _mockUserService.Setup(s => s.Delete<DeveloperContract, Developer>("userId"))
                .ReturnsAsync(developerContract);

            // Act
            var result = await _controller.Delete(token) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(developerContract));
        }

        [Test]
        public async Task Delete_WithCustomerRole_ReturnsOk()
        {
            // Arrange
            var token = "Bearer testToken";
            var customerContract = new CustomerContract();

            _mockAuthService.Setup(s => s.GetId("testToken")).Returns("userId");
            _mockAuthService.Setup(s => s.GetRole("testToken")).Returns("Customer");
            _mockUserService.Setup(s => s.Delete<CustomerContract, Customer>("userId"))
                .ReturnsAsync(customerContract);

            // Act
            var result = await _controller.Delete(token) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(customerContract));
        }

        [Test]
        public async Task Delete_WithUnknownRole_ReturnsOk()
        {
            // Arrange
            var token = "Bearer testToken";
            var userContract = new UserContract();

            _mockAuthService.Setup(s => s.GetId("testToken")).Returns("userId");
            _mockAuthService.Setup(s => s.GetRole("testToken")).Returns("Unknown");
            _mockUserService.Setup(s => s.Delete<UserContract, User>("userId"))
                .ReturnsAsync(userContract);

            // Act
            var result = await _controller.Delete(token) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(userContract));
        }

        [Test]
        public async Task Delete_WithInvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var token = "InvalidToken";

            // Act
            var result = await _controller.Delete(token);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task Delete_WithNullUserId_ReturnsBadRequest()
        {
            // Arrange
            var token = "Bearer testToken";
            _mockAuthService.Setup(s => s.GetId("testToken")).Returns((string?)null);

            // Act
            var result = await _controller.Delete(token);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task DeleteAdmin_ReturnsOk()
        {
            // Arrange
            var adminContract = new AdminContract();
            _mockUserService.Setup(s => s.Delete<AdminContract, Admin>("adminId"))
                .ReturnsAsync(adminContract);

            // Act
            var result = await _controller.DeleteAdmin("adminId") as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(adminContract));
        }
    }
}