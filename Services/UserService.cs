using System;
using AutoMapper;
using BCrypt.Net;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;
using Indiego_Backend.Repositories;
using Indiego_Backend.Utilities;
using Microsoft.IdentityModel.Tokens;

namespace Indiego_Backend.Services;

public interface IUserService
{
    Task<TResponse?> GetLoggedInUser<TResponse, TEntity>(string token)
        where TResponse : UserContract
        where TEntity : User;

    Task<LoginResponseContract?> Login(LoginContract loginContract);

    Task<List<TResponse>> Get<TResponse, TEntity>(string? id = null, string? email = null)
        where TResponse : UserContract
        where TEntity : User;

    Task<TResponse?> Create<TResponse, TEntity, TCreate>(TCreate create)
        where TResponse : UserContract
        where TEntity : User
        where TCreate : CreateUserContract;

    Task<TResponse?> Update<TResponse, TEntity, TUpdate>(string id, TUpdate update)
        where TResponse : UserContract
        where TEntity : User
        where TUpdate : UpdateUserContract;

    Task<TResponse?> Delete<TResponse, TEntity>(string id)
        where TResponse : UserContract
        where TEntity : User;
    Task<DeveloperContract?> ConvertCustomerToDeveloper(string token);
    Task AddReview(string token, string reviewId);
    Task RemoveReview(string token, string reviewId);
    Task AddLike(string token, string postId);
    Task RemoveLike(string token, string postId);
    Task AddFavorite(string token, string gameId);
    Task RemoveFavorite(string token, string gameId);
    Task Download(string token, string gameId);
    Task GameCreated(string token, string gameId);
    Task GameDeleted(string token, string gameId);
    Task PostCreated(string token, string postId);
    Task PostDeleted(string token, string postId);
    Task AddBalance(string token, int amount);
    Task RemoveBalance(string token, int amount);
    Task Subscribe(string token, string subscriptionId);
    Task Unsubscribe(string token);
}

public class UserService(
    IUserRepository<User> userRepository,
    IUserRepository<Admin> adminRepository,
    IUserRepository<Customer> customerRepository,
    IUserRepository<Developer> developerRepository,
    IAuthenticationService authenticationService,
    IMapper mapper
) : IUserService
{
    private readonly IUserRepository<User> _userRepository = userRepository;
    private readonly IUserRepository<Admin> _adminRepository = adminRepository;
    private readonly IUserRepository<Customer> _customerRepository = customerRepository;
    private readonly IUserRepository<Developer> _developerRepository = developerRepository;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IMapper _mapper = mapper;

    public async Task<TResponse?> GetLoggedInUser<TResponse, TEntity>(string token)
        where TResponse : UserContract
        where TEntity : User
    {
        var userId = _authenticationService.GetId(token);
        if (userId == null) return null;

        IUserRepository<TEntity> repository;
        if (typeof(TEntity) == typeof(Admin))
            repository = (IUserRepository<TEntity>)_adminRepository;
        else if (typeof(TEntity) == typeof(Developer))
            repository = (IUserRepository<TEntity>)_developerRepository;
        else if (typeof(TEntity) == typeof(Customer))
            repository = (IUserRepository<TEntity>)_customerRepository;
        else
            repository = (IUserRepository<TEntity>)_userRepository;

        var user = (await repository.Get(userId, null)).FirstOrDefault();
        if (user == null) return null;
        return _mapper.Map<TResponse>(user);
    }

    public async Task<LoginResponseContract?> Login(LoginContract loginContract)
    {
        var user = (await _userRepository.Get(null, loginContract.Email)).FirstOrDefault();
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginContract.Password, user.Password)) return null;
        return new LoginResponseContract
        {
            Token = _authenticationService.GenerateToken(user),
            User = _mapper.Map<UserContract>(user)
        };
    }

    public async Task<List<TResponse>> Get<TResponse, TEntity>(string? id = null, string? email = null)
        where TResponse : UserContract
        where TEntity : User
    {
        IUserRepository<TEntity> repository;
        if (typeof(TEntity) == typeof(Admin))
            repository = (IUserRepository<TEntity>)_adminRepository;
        else if (typeof(TEntity) == typeof(Developer))
            repository = (IUserRepository<TEntity>)_developerRepository;
        else if (typeof(TEntity) == typeof(Customer))
            repository = (IUserRepository<TEntity>)_customerRepository;
        else
            repository = (IUserRepository<TEntity>)_userRepository;
        return _mapper.Map<List<TResponse>>(await repository.Get(id, email));
    }

    public async Task<TResponse?> Create<TResponse, TEntity, TCreate>(TCreate create)
        where TResponse : UserContract
        where TEntity : User
        where TCreate : CreateUserContract
    {
        IUserRepository<TEntity> repository;
        if (typeof(TEntity) == typeof(Admin))
            repository = (IUserRepository<TEntity>)_adminRepository;
        else if (typeof(TEntity) == typeof(Developer))
            repository = (IUserRepository<TEntity>)_developerRepository;
        else if (typeof(TEntity) == typeof(Customer))
            repository = (IUserRepository<TEntity>)_customerRepository;
        else
            repository = (IUserRepository<TEntity>)_userRepository;
        var user = _mapper.Map<TEntity>(create);
        user.Password = BCrypt.Net.BCrypt.HashPassword(create.Password);
        await repository.Create(user);
        return _mapper.Map<TResponse>(user);
    }

    public async Task<TResponse?> Update<TResponse, TEntity, TUpdate>(string id, TUpdate update)
        where TResponse : UserContract
        where TEntity : User
        where TUpdate : UpdateUserContract
    {
        IUserRepository<TEntity> repository;
        if (typeof(TEntity) == typeof(Admin))
            repository = (IUserRepository<TEntity>)_adminRepository;
        else if (typeof(TEntity) == typeof(Developer))
            repository = (IUserRepository<TEntity>)_developerRepository;
        else if (typeof(TEntity) == typeof(Customer))
            repository = (IUserRepository<TEntity>)_customerRepository;
        else
            repository = (IUserRepository<TEntity>)_userRepository;
        var user = (await repository.Get(id)).FirstOrDefault();
        if (user == null) return null;
        return _mapper.Map<TResponse>(await repository.Update(id, _mapper.Map(update, user)));
    }

    public async Task<TResponse?> Delete<TResponse, TEntity>(string id)
        where TResponse : UserContract
        where TEntity : User
    {
        IUserRepository<TEntity> repository;
        if (typeof(TEntity) == typeof(Admin))
            repository = (IUserRepository<TEntity>)_adminRepository;
        else if (typeof(TEntity) == typeof(Developer))
            repository = (IUserRepository<TEntity>)_developerRepository;
        else if (typeof(TEntity) == typeof(Customer))
            repository = (IUserRepository<TEntity>)_customerRepository;
        else
            repository = (IUserRepository<TEntity>)_userRepository;
        var user = await repository.Delete(id);
        return _mapper.Map<TResponse>(user);
    }

    public async Task<DeveloperContract?> ConvertCustomerToDeveloper(string token)
    {
        var userId = _authenticationService.GetId(token);
        if (userId == null) return null;

        var customer = (await _customerRepository.Get(userId, null)).FirstOrDefault();
        if (customer == null) return null;

        var developer = new Developer()
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Password = customer.Password,
            SubscriptionId = customer.SubscriptionId,
            ReviewIds = customer.ReviewIds,
            Likes = customer.Likes,
            Favorites = customer.Favorites,
            Downloads = customer.Downloads,
            BirthDate = customer.BirthDate,
        };
        await _customerRepository.Delete(userId);
        await _developerRepository.Create(developer);
        return _mapper.Map<DeveloperContract>(developer);
    }

    public async Task AddReview(string token, string reviewId)
    {
        var userId = _authenticationService.GetId(token);
        if (userId == null) return;

        var customer = (await _customerRepository.Get(userId, null)).FirstOrDefault();
        if (customer == null) return;

        customer.ReviewIds.Add(reviewId);
        await _customerRepository.Update(userId, customer);
    }

    public async Task RemoveReview(string token, string reviewId)
    {
        var userId = _authenticationService.GetId(token);
        if (userId == null) return;

        var customer = (await _customerRepository.Get(userId, null)).FirstOrDefault();
        if (customer == null) return;

        customer.ReviewIds.Remove(reviewId);
        await _customerRepository.Update(userId, customer);
    }

    public async Task AddLike(string token, string postId)
    {
        var userId = _authenticationService.GetId(token);
        if (userId == null) return;

        var customer = (await _customerRepository.Get(userId, null)).FirstOrDefault();
        if (customer == null) return;

        customer.Likes.Add(postId);
        await _customerRepository.Update(userId, customer);
    }

    public async Task RemoveLike(string token, string postId)
    {
        var userId = _authenticationService.GetId(token);
        if (userId == null) return;

        var customer = (await _customerRepository.Get(userId, null)).FirstOrDefault();
        if (customer == null) return;

        customer.Likes.Remove(postId);
        await _customerRepository.Update(userId, customer);
    }

    public async Task AddFavorite(string token, string gameId)
    {
        var userId = _authenticationService.GetId(token);
        if (userId == null) return;

        var customer = (await _customerRepository.Get(userId, null)).FirstOrDefault();
        if (customer == null) return;

        customer.Favorites.Add(gameId);
        await _customerRepository.Update(userId, customer);
    }

    public async Task RemoveFavorite(string token, string gameId)
    {
        var userId = _authenticationService.GetId(token);
        if (userId == null) return;

        var customer = (await _customerRepository.Get(userId, null)).FirstOrDefault();
        if (customer == null) return;

        customer.Favorites.Remove(gameId);
        await _customerRepository.Update(userId, customer);
    }

    public async Task Download(string token, string gameId)
    {
        var userId = _authenticationService.GetId(token);
        if (userId == null) return;

        var customer = (await _customerRepository.Get(userId, null)).FirstOrDefault();
        if (customer == null) return;

        var download = new Download
        {
            UserId = userId,
            GameId = gameId,
        };
        if (!customer.Downloads.Any(d => d.UserId == userId && d.GameId == gameId))customer.Downloads.Add(download);
        await _customerRepository.Update(userId, customer);
    }

    public async Task GameCreated(string token, string gameId)
    {
        var userId = _authenticationService.GetId(token);
        if (userId == null) return;

        var developer = (await _developerRepository.Get(userId, null)).FirstOrDefault();
        if (developer == null) return;

        developer.GameIds.Add(gameId);
        await _developerRepository.Update(userId, developer);
    }

    public async Task GameDeleted(string token, string gameId)
    {
        var userId = _authenticationService.GetId(token);
        if (userId == null) return;

        var developer = (await _developerRepository.Get(userId, null)).FirstOrDefault();
        if (developer == null) return;

        developer.GameIds.Remove(gameId);
        await _developerRepository.Update(userId, developer);
    }

    public async Task PostCreated(string token, string postId)
    {
        var userId = _authenticationService.GetId(token);
        if (userId == null) return;

        var developer = (await _developerRepository.Get(userId, null)).FirstOrDefault();
        if (developer == null) return;

        developer.PostIds.Add(postId);
        await _developerRepository.Update(userId, developer);
    }

    public async Task PostDeleted(string token, string postId)
    {
        var userId = _authenticationService.GetId(token);
        if (userId == null) return;

        var developer = (await _developerRepository.Get(userId, null)).FirstOrDefault();
        if (developer == null) return;

        developer.PostIds.Remove(postId);
        await _developerRepository.Update(userId, developer);
    }

    public async Task AddBalance(string token, int amount)
    {
        var userId = _authenticationService.GetId(token);
        if (userId == null) return;

        var developer = (await _developerRepository.Get(userId, null)).FirstOrDefault();
        if (developer == null) return;

        developer.Balance += amount;
        await _developerRepository.Update(userId, developer);
    }

    public async Task RemoveBalance(string token, int amount)
    {
        var userId = _authenticationService.GetId(token);
        if (userId == null) return;

        var developer = (await _developerRepository.Get(userId, null)).FirstOrDefault();
        if (developer == null) return;

        developer.Balance -= amount;
        await _developerRepository.Update(userId, developer);
    }

    public async Task Subscribe(string token, string subscriptionId)
    {
        var userId = _authenticationService.GetId(token);
        if (userId == null) return;

        var customer = (await _customerRepository.Get(userId, null)).FirstOrDefault();
        if (customer == null) return;

        customer.SubscriptionId = subscriptionId;
        await _customerRepository.Update(userId, customer);
    }

    public async Task Unsubscribe(string token)
    {
        var userId = _authenticationService.GetId(token);
        if (userId == null) return;

        var customer = (await _customerRepository.Get(userId, null)).FirstOrDefault();
        if (customer == null) return;

        customer.SubscriptionId = null;
        await _customerRepository.Update(userId, customer);
    }
}
