using AutoMapper;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;
using Indiego_Backend.Repositories;
using Indiego_Backend.Services;
using Moq;
using NUnit.Framework;

namespace Indiego_Backend.Tests
{
    public class UserServiceTest
    {
        private Mock<IUserRepository<User>> _mockUserRepository = null!;
        private Mock<IUserRepository<Admin>> _mockAdminRepository = null!;
        private Mock<IUserRepository<Customer>> _mockCustomerRepository = null!;
        private Mock<IUserRepository<Developer>> _mockDeveloperRepository = null!;
        private Mock<IGameService> _mockGameService = null!;
        private Mock<IGenreService> _mockGenreService = null!;
        private Mock<IPostService> _mockPostService = null!;
        private Mock<IReviewService> _mockReviewService = null!;
        private Mock<IAuthenticationService> _mockAuthService = null!;
        private Mock<IMapper> _mockMapper = null!;
        private UserService _userService = null!;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository<User>>();
            _mockAdminRepository = new Mock<IUserRepository<Admin>>();
            _mockCustomerRepository = new Mock<IUserRepository<Customer>>();
            _mockDeveloperRepository = new Mock<IUserRepository<Developer>>();
            _mockGameService = new Mock<IGameService>();
            _mockGenreService = new Mock<IGenreService>();
            _mockPostService = new Mock<IPostService>();
            _mockReviewService = new Mock<IReviewService>();
            _mockAuthService = new Mock<IAuthenticationService>();
            _mockMapper = new Mock<IMapper>();

            _userService = new UserService(
                _mockUserRepository.Object,
                _mockAdminRepository.Object,
                _mockCustomerRepository.Object,
                _mockDeveloperRepository.Object,
                _mockGameService.Object,
                _mockGenreService.Object,
                _mockPostService.Object,
                _mockReviewService.Object,
                _mockAuthService.Object,
                _mockMapper.Object
            );
        }

        [Test]
        public async Task Login_WithValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new Customer { Id = "userId", Email = email, Password = hashedPassword };
            var userContract = new UserContract { Id = "userId", Email = email };
            var loginContract = new LoginContract { Email = email, Password = password };
            var token = "test-token";

            _mockUserRepository.Setup(repo => repo.Get(null, email))
                .ReturnsAsync([user]);
            _mockAuthService.Setup(auth => auth.GenerateToken(user))
                .Returns(token);
            _mockMapper.Setup(mapper => mapper.Map<UserContract>(user))
                .Returns(userContract);

            // Act
            var result = await _userService.Login(loginContract);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Token, Is.EqualTo(token));
            Assert.That(result.User, Is.EqualTo(userContract));
        }

        [Test]
        public async Task Login_WithInvalidCredentials_ReturnsNull()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("differentPassword");
            var user = new Customer { Id = "userId", Email = email, Password = hashedPassword };
            var loginContract = new LoginContract { Email = email, Password = password };

            _mockUserRepository.Setup(repo => repo.Get(null, email))
                .ReturnsAsync([user]);

            // Act
            var result = await _userService.Login(loginContract);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetLoggedInUser_WithValidToken_ReturnsUser()
        {
            // Arrange
            var token = "valid-token";
            var userId = "userId";
            var user = new Customer { Id = userId, Email = "test@example.com" };
            var userContract = new CustomerContract { Id = userId, Email = "test@example.com" };

            _mockAuthService.Setup(auth => auth.GetId(token))
                .Returns(userId);
            _mockCustomerRepository.Setup(repo => repo.Get(userId, null))
                .ReturnsAsync([user]);
            _mockMapper.Setup(mapper => mapper.Map<CustomerContract>(user))
                .Returns(userContract);

            // Act
            var result = await _userService.GetLoggedInUser<CustomerContract, Customer>(token);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(userId));
        }

        [Test]
        public async Task GetLoggedInUser_WithInvalidToken_ReturnsNull()
        {
            // Arrange
            var token = "invalid-token";

            _mockAuthService.Setup(auth => auth.GetId(token))
                .Returns((string)null!);

            // Act
            var result = await _userService.GetLoggedInUser<CustomerContract, Customer>(token);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Create_WithValidData_ReturnsCreatedUser()
        {
            // Arrange
            var createContract = new CreateUserContract 
            { 
            Email = "new@example.com", 
            Password = "password123", 
            Name = "New User" 
            };
            var customer = new Customer 
            { 
            Id = "newUserId", 
            Email = "new@example.com", 
            Name = "New User" 
            };
            var customerContract = new CustomerContract 
            { 
            Id = "newUserId", 
            Email = "new@example.com", 
            Name = "New User" 
            };

            _mockMapper.Setup(m => m.Map<Customer>(createContract)).Returns(customer);
            _mockCustomerRepository.Setup(r => r.Create(It.IsAny<Customer>())).ReturnsAsync(customer);
            _mockMapper.Setup(m => m.Map<CustomerContract>(customer)).Returns(customerContract);

            // Act
            var result = await _userService.Create<CustomerContract, Customer, CreateUserContract>(createContract);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo("newUserId"));
            Assert.That(result.Email, Is.EqualTo("new@example.com"));
            _mockCustomerRepository.Verify(r => r.Create(It.IsAny<Customer>()), Times.Once);
        }

        [Test]
        public async Task Get_WithId_ReturnsMatchingUser()
        {
            // Arrange
            var userId = "userId";
            var customer = new Customer { Id = userId, Email = "test@example.com" };
            var customerContract = new CustomerContract { Id = userId, Email = "test@example.com" };

            _mockCustomerRepository.Setup(r => r.Get(userId, null)).ReturnsAsync([customer]);
            _mockMapper.Setup(m => m.Map<List<CustomerContract>>(It.IsAny<List<Customer>>())).Returns([customerContract]);

            // Act
            var result = await _userService.Get<CustomerContract, Customer>(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(userId));
        }

        [Test]
        public async Task Get_WithInvalidId_ReturnsEmptyList()
        {
            // Arrange
            var userId = "nonExistentId";
            
            _mockCustomerRepository.Setup(r => r.Get(userId, null)).ReturnsAsync(new List<Customer>());
            _mockMapper.Setup(m => m.Map<List<CustomerContract>>(It.IsAny<List<Customer>>())).Returns(new List<CustomerContract>());

            // Act
            var result = await _userService.Get<CustomerContract, Customer>(userId);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task Get_WithEmail_ReturnsMatchingUsers()
        {
            // Arrange
            var email = "test@example.com";
            var customer = new Customer { Id = "userId", Email = email };
            var customerContract = new CustomerContract { Id = "userId", Email = email };

            _mockCustomerRepository.Setup(r => r.Get(null, email)).ReturnsAsync([customer]);
            _mockMapper.Setup(m => m.Map<List<CustomerContract>>(It.IsAny<List<Customer>>())).Returns([customerContract]);

            // Act
            var result = await _userService.Get<CustomerContract, Customer>(null, email);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Email, Is.EqualTo(email));
        }

        [Test]
        public async Task Get_WithInvalidEmail_ReturnsEmptyList()
        {
            // Arrange
            var email = "nonexistent@example.com";
            
            _mockCustomerRepository.Setup(r => r.Get(null, email)).ReturnsAsync(new List<Customer>());
            _mockMapper.Setup(m => m.Map<List<CustomerContract>>(It.IsAny<List<Customer>>())).Returns(new List<CustomerContract>());

            // Act
            var result = await _userService.Get<CustomerContract, Customer>(null, email);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task Update_WithValidData_ReturnsUpdatedUser()
        {
            // Arrange
            var userId = "userId";
            var updateContract = new UpdateUserContract { Name = "Updated Name" };
            var existingCustomer = new Customer { Id = userId, Name = "Old Name", Email = "test@example.com" };
            var updatedCustomer = new Customer { Id = userId, Name = "Updated Name", Email = "test@example.com" };
            var customerContract = new CustomerContract { Id = userId, Name = "Updated Name", Email = "test@example.com" };

            _mockCustomerRepository.Setup(r => r.Get(userId, null)).ReturnsAsync([existingCustomer]);
            _mockMapper.Setup(m => m.Map(updateContract, existingCustomer)).Returns(updatedCustomer);
            _mockCustomerRepository.Setup(r => r.Update(userId, It.IsAny<Customer>())).ReturnsAsync(updatedCustomer);
            _mockMapper.Setup(m => m.Map<CustomerContract>(updatedCustomer)).Returns(customerContract);

            // Act
            var result = await _userService.Update<CustomerContract, Customer, UpdateUserContract>(userId, updateContract);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(userId));
            Assert.That(result.Name, Is.EqualTo("Updated Name"));
            _mockCustomerRepository.Verify(r => r.Update(userId, It.IsAny<Customer>()), Times.Once);
        }

        [Test]
        public async Task Update_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var userId = "nonExistentId";
            var updateContract = new UpdateUserContract { Name = "Updated Name" };

            _mockCustomerRepository.Setup(r => r.Get(userId, null)).ReturnsAsync(new List<Customer>());

            // Act
            var result = await _userService.Update<CustomerContract, Customer, UpdateUserContract>(userId, updateContract);

            // Assert
            Assert.That(result, Is.Null);
            _mockCustomerRepository.Verify(r => r.Update(It.IsAny<string>(), It.IsAny<Customer>()), Times.Never);
        }

        [Test]
        public async Task Delete_WithValidId_ReturnsDeletedUser()
        {
            // Arrange
            var userId = "userId";
            var customer = new Customer { Id = userId, Email = "test@example.com" };
            var customerContract = new CustomerContract { Id = userId, Email = "test@example.com" };

            _mockCustomerRepository.Setup(r => r.Delete(userId)).ReturnsAsync(customer);
            _mockMapper.Setup(m => m.Map<CustomerContract>(customer)).Returns(customerContract);

            // Act
            var result = await _userService.Delete<CustomerContract, Customer>(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(userId));
            _mockCustomerRepository.Verify(r => r.Delete(userId), Times.Once);
        }

        [Test]
        public async Task Delete_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var userId = "nonExistentId";

            _mockCustomerRepository.Setup(r => r.Delete(userId)).ReturnsAsync((Customer)null!);

            // Act
            var result = await _userService.Delete<CustomerContract, Customer>(userId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task ConvertCustomerToDeveloper_ValidCustomer_ReturnsDeveloperContract()
        {
            // Arrange
            var token = "valid-token";
            var userId = "userId";
            var customer = new Customer 
            { 
                Id = userId, 
                Name = "Test User", 
                Email = "test@example.com",
                Password = "hashedPassword" 
            };
            var developer = new Developer
            {
                Id = userId,
                Name = "Test User",
                Email = "test@example.com",
                Password = "hashedPassword"
            };
            var developerContract = new DeveloperContract { Id = userId, Email = "test@example.com" };

            _mockAuthService.Setup(auth => auth.GetId(token))
                .Returns(userId);
            _mockCustomerRepository.Setup(repo => repo.Get(userId, null))
                .ReturnsAsync([customer]);
            _mockCustomerRepository.Setup(repo => repo.Delete(userId))
                .ReturnsAsync(customer);
            _mockDeveloperRepository.Setup(repo => repo.Create(It.IsAny<Developer>()))
                .ReturnsAsync(developer);
            _mockMapper.Setup(mapper => mapper.Map<DeveloperContract>(It.IsAny<Developer>()))
                .Returns(developerContract);

            // Act
            var result = await _userService.ConvertCustomerToDeveloper(token);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(userId));
            _mockCustomerRepository.Verify(repo => repo.Delete(userId), Times.Once);
            _mockDeveloperRepository.Verify(repo => repo.Create(It.IsAny<Developer>()), Times.Once);
        }

        [Test]
        public async Task ConvertCustomerToDeveloper_InvalidCustomer_ReturnsNull()
        {
            // Arrange
            var token = "valid-token";
            var userId = "userId";

            _mockAuthService.Setup(auth => auth.GetId(token))
                .Returns(userId);
            _mockCustomerRepository.Setup(repo => repo.Get(userId, null))
                .ReturnsAsync([]);

            // Act
            var result = await _userService.ConvertCustomerToDeveloper(token);

            // Assert
            Assert.That(result, Is.Null);
            _mockCustomerRepository.Verify(repo => repo.Delete(It.IsAny<string>()), Times.Never);
            _mockDeveloperRepository.Verify(repo => repo.Create(It.IsAny<Developer>()), Times.Never);
        }
    }
}