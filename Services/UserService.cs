using System;
using AutoMapper;
using Indiego_Backend.Contracts.Users;
using Indiego_Backend.Models.Users;
using Indiego_Backend.Repositories;

namespace Indiego_Backend.Services;

public interface IUserService
{
    Task<User?> GetLoggedInUser(string token);
    Task<TokenContract?> Login(LoginContract credential);
    Task<List<TResponse>> Get<TResponse, TEntity>(string? id = null, string? email = null)
        where TResponse : UserContract
        where TEntity : User;
    Task<TResponse?> Create<TResponse, TEntity, TCreate>(TCreate create)
        where TResponse : UserContract
        where TEntity : User
        where TCreate : CreateUserContract;
    Task<DeveloperContract?> ConvertCustomerToDeveloper(string id, CreateDeveloperContract contract);
    Task<TResponse?> Update<TResponse, TEntity, TUpdate>(string id, TUpdate update)
        where TResponse : UserContract
        where TEntity : User
        where TUpdate : UpdateUserContract;
    Task<TResponse?> Delete<TResponse>(string id) where TResponse : UserContract;
}

public class UserService(IUserRepository<User> userRepository, IUserRepository<Admin> adminRepository, IUserRepository<Customer> customerRepository, IUserRepository<Developer> developerRepository, IMapper mapper, AuthService auth) : IUserService
{
    private readonly IUserRepository<User> _user = userRepository;
    private readonly IUserRepository<Admin> _admin = adminRepository;
    private readonly IUserRepository<Customer> _customer = customerRepository;
    private readonly IUserRepository<Developer> _developer = developerRepository;
    private readonly IMapper _mapper = mapper;
    private readonly AuthService _auth = auth;

    public async Task<User?> GetLoggedInUser(string token)
    {
        var tokenArr = token.Split(' ');
        var user = _auth.GetId(tokenArr[1]);
        if (user == null) return null;
        return (await _user.Get(user, null))[0];
    }

    public async Task<TokenContract?> Login(LoginContract credential)
    {
        var user = (await _user.Get(null, credential.Email))[0];
        if (user == null || !BCrypt.Net.BCrypt.Verify(credential.Password, user.Password)) return null;
        return new TokenContract
        {
            Token = _auth.GenerateToken(user),
            User = _mapper.Map<UserContract>(user)
        };
    }

    public async Task<List<TResponse>> Get<TResponse, TEntity>(string? id = null, string? email = null) 
        where TResponse : UserContract
        where TEntity : User
    {
        IUserRepository<TEntity> repository;
        if (typeof(TEntity) == typeof(Admin))
            repository = (IUserRepository<TEntity>)_admin;
        else if (typeof(TEntity) == typeof(Customer))
            repository = (IUserRepository<TEntity>)_customer;
        else if (typeof(TEntity) == typeof(Developer))
            repository = (IUserRepository<TEntity>)_developer;
        else
            repository = (IUserRepository<TEntity>)_user;
        
        return _mapper.Map<List<TResponse>>(await repository.Get(id, email));
    }

    public async Task<TResponse?> Create<TResponse, TEntity, TCreate>(TCreate create) 
        where TResponse : UserContract
        where TEntity : User
        where TCreate : CreateUserContract
    {
        var user = await _user.Get(null, create.Email);
        if (user.Count > 0) return null;
        
        var entity = _mapper.Map<TEntity>(create);
        entity.Password = BCrypt.Net.BCrypt.HashPassword(create.Password);
        
        IUserRepository<TEntity> repository;
        if (typeof(TEntity) == typeof(Admin))
            repository = (IUserRepository<TEntity>)_admin;
        else if (typeof(TEntity) == typeof(Customer))
            repository = (IUserRepository<TEntity>)_customer;
        else if (typeof(TEntity) == typeof(Developer))
            repository = (IUserRepository<TEntity>)_developer;
        else
            repository = (IUserRepository<TEntity>)_user;

        entity = await repository.Create(entity);
        return _mapper.Map<TResponse>(entity);
    }

    public async Task<DeveloperContract?> ConvertCustomerToDeveloper(string id, CreateDeveloperContract contract)
    {
        var customer = (await _customer.Get(id, null)).FirstOrDefault();
        if (customer == null)
            return null;
        Developer developer = new()
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Password = customer.Password,
            BirthDate = customer.BirthDate,
            Bank = contract.Bank,
            AccountNumber = contract.AccountNumber,
            AccountName = contract.AccountName
        };
        await _customer.Delete(id);
        await _developer.Create(developer);
        return _mapper.Map<DeveloperContract>(developer);
    }

    public async Task<TResponse?> Update<TResponse, TEntity, TUpdate>(string id, TUpdate update)
        where TResponse : UserContract
        where TEntity : User
        where TUpdate : UpdateUserContract
    {
        IUserRepository<TEntity> repository;
        if (typeof(TEntity) == typeof(Admin))
            repository = (IUserRepository<TEntity>)_admin;
        else if (typeof(TEntity) == typeof(Customer))
            repository = (IUserRepository<TEntity>)_customer;
        else if (typeof(TEntity) == typeof(Developer))
            repository = (IUserRepository<TEntity>)_developer;
        else
            repository = (IUserRepository<TEntity>)_user;
        var entity = (await repository.Get(id))[0];
        if (entity == null) return null;
        entity = await repository.Update(id, _mapper.Map<TUpdate, TEntity>(update, entity));
        return _mapper.Map<TResponse>(entity);
    }

    public async Task<TResponse?> Delete<TResponse>(string id) where TResponse : UserContract
    {
        var entity = await _user.Delete(id);
        if (entity == null) return null;
        return _mapper.Map<TResponse>(entity);
    }
}
 