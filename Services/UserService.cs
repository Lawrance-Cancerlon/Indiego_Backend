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
    Task AddReview(string id, string reviewId);
    Task RemoveReview(string id, string reviewId);
    Task AddLike(string id, string postId);
    Task RemoveLike(string id, string postId);
    Task AddFavorite(string id, string gameId);
    Task RemoveFavorite(string id, string gameId);
    Task Download(string id, string gameId);
    Task AddGame(string id, string gameId);
    Task RemoveGame(string id, string gameId);
    Task AddPost(string id, string postId);
    Task RemovePost(string id, string postId);
    Task AddBalance(string id, int amount);
    Task RemoveBalance(string id, int amount);
    Task Subscribe(string id, string subscriptionId);
    Task Unsubscribe(string id);
}

public class UserService(
    IUserRepository<User> userRepository,
    IUserRepository<Admin> adminRepository,
    IUserRepository<Customer> customerRepository,
    IUserRepository<Developer> developerRepository,
    IGameService gameService,
    IPostService postService,
    IAuthenticationService authenticationService,
    IMapper mapper
) : IUserService
{
    private readonly IUserRepository<User> _userRepository = userRepository;
    private readonly IUserRepository<Admin> _adminRepository = adminRepository;
    private readonly IUserRepository<Customer> _customerRepository = customerRepository;
    private readonly IUserRepository<Developer> _developerRepository = developerRepository;
    private readonly IGameService _gameService = gameService;
    private readonly IPostService _postService = postService;
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
        if (user != null && typeof(TEntity) == typeof(Developer))
        {
            var developer = (Developer)(object)user;
            foreach (var game in developer.GameIds) await _gameService.Delete(game);
            foreach (var post in developer.PostIds) await _postService.Delete(post);
        }
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

    public async Task AddReview(string id, string reviewId)
    {
        var customer = (await _customerRepository.Get(id, null)).FirstOrDefault();
        if (customer == null) return;

        if(!customer.ReviewIds.Contains(reviewId)) customer.ReviewIds.Add(reviewId);
        await _customerRepository.Update(id, customer);
    }

    public async Task RemoveReview(string id, string reviewId)
    {
        var customer = (await _customerRepository.Get(id, null)).FirstOrDefault();
        if (customer == null) return;

        customer.ReviewIds.Remove(reviewId);
        await _customerRepository.Update(id, customer);
    }

    public async Task AddLike(string id, string postId)
    {
        var customer = (await _customerRepository.Get(id, null)).FirstOrDefault();
        if (customer == null) return;

        if(!customer.Likes.Contains(postId)) customer.Likes.Add(postId);
        await _customerRepository.Update(id, customer);
    }

    public async Task RemoveLike(string id, string postId)
    {
        var customer = (await _customerRepository.Get(id, null)).FirstOrDefault();
        if (customer == null) return;

        customer.Likes.Remove(postId);
        await _customerRepository.Update(id, customer);
    }

    public async Task AddFavorite(string id, string gameId)
    {
        var customer = (await _customerRepository.Get(id, null)).FirstOrDefault();
        if (customer == null) return;

        if(!customer.Favorites.Contains(gameId)) customer.Favorites.Add(gameId);
        await _customerRepository.Update(id, customer);
    }

    public async Task RemoveFavorite(string id, string gameId)
    {
        var customer = (await _customerRepository.Get(id, null)).FirstOrDefault();
        if (customer == null) return;

        customer.Favorites.Remove(gameId);
        await _customerRepository.Update(id, customer);
    }

    public async Task Download(string id, string gameId)
    {
        var customer = (await _customerRepository.Get(id, null)).FirstOrDefault();
        if (customer == null) return;

        var download = new Download
        {
            UserId = id,
            GameId = gameId,
        };
        if (!customer.Downloads.Any(d => d.UserId == id && d.GameId == gameId))customer.Downloads.Add(download);
        await _customerRepository.Update(id, customer);
    }

    public async Task AddGame(string id, string gameId)
    {
        var developer = (await _developerRepository.Get(id, null)).FirstOrDefault();
        if (developer == null) return;

        if(!developer.GameIds.Contains(gameId)) developer.GameIds.Add(gameId);
        await _developerRepository.Update(id, developer);
    }

    public async Task RemoveGame(string id, string gameId)
    {
        var developer = (await _developerRepository.Get(id, null)).FirstOrDefault();
        if (developer == null) return;

        developer.GameIds.Remove(gameId);
        await _developerRepository.Update(id, developer);
    }

    public async Task AddPost(string id, string postId)
    {
        var developer = (await _developerRepository.Get(id, null)).FirstOrDefault();
        if (developer == null) return;

        if(!developer.PostIds.Contains(postId)) developer.PostIds.Add(postId);
        await _developerRepository.Update(id, developer);
    }

    public async Task RemovePost(string id, string postId)
    {
        var developer = (await _developerRepository.Get(id, null)).FirstOrDefault();
        if (developer == null) return;

        developer.PostIds.Remove(postId);
        await _developerRepository.Update(id, developer);
    }

    public async Task AddBalance(string id, int amount)
    {
        var developer = (await _developerRepository.Get(id, null)).FirstOrDefault();
        if (developer == null) return;

        developer.Balance += amount;
        await _developerRepository.Update(id, developer);
    }

    public async Task RemoveBalance(string id, int amount)
    {
        var developer = (await _developerRepository.Get(id, null)).FirstOrDefault();
        if (developer == null) return;

        developer.Balance -= amount;
        await _developerRepository.Update(id, developer);
    }

    public async Task Subscribe(string id, string subscriptionId)
    {
        var customer = (await _customerRepository.Get(id, null)).FirstOrDefault();
        if (customer == null) return;

        customer.SubscriptionId = subscriptionId;
        await _customerRepository.Update(id, customer);
    }

    public async Task Unsubscribe(string id)
    {
        var customer = (await _customerRepository.Get(id, null)).FirstOrDefault();
        if (customer == null) return;

        customer.SubscriptionId = null;
        await _customerRepository.Update(id, customer);
    }
}
