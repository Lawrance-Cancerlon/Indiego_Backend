using System;
using AutoMapper;
using BCrypt.Net;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;
using Indiego_Backend.Repositories;
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
        if(userId == null) return null;
        
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
        var user = await _userRepository.Get(null, loginContract.Email);
        if (user.IsNullOrEmpty() || !BCrypt.Net.BCrypt.Verify(loginContract.Password, user[0].Password)) return null;
        return new LoginResponseContract
        {
            Token = _authenticationService.GenerateToken(user[0]),
            User = _mapper.Map<UserContract>(user[0])
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
        var user = (await repository.Get(id))[0];
        if (user == null) return null;
        user = await repository.Update(id, _mapper.Map(update, user));
        return _mapper.Map<TResponse>(user);
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
        if(userId == null) return null;
        
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
            Dislikes = customer.Dislikes,
            Favorites = customer.Favorites,
            Downloads = customer.Downloads,
            BirthDate = customer.BirthDate,
        };
        await _customerRepository.Delete(userId);
        await _developerRepository.Create(developer);
        return _mapper.Map<DeveloperContract>(developer);
    }
}
