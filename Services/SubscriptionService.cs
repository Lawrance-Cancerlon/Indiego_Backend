using System;
using AutoMapper;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;
using Indiego_Backend.Repositories;

namespace Indiego_Backend.Services;

public interface ISubscriptionService
{
    Task<List<SubscriptionContract>> Get(string? id = null, string? userId = null);
    Task<SubscriptionContract?> Create(CreateSubscriptionContract create, string token);
    Task<SubscriptionContract?> Delete(string id);
}

public class SubscriptionService(ISubscriptionRepository repository, IUserService userService, IAuthenticationService authenticationService, IMapper mapper) : ISubscriptionService
{
    private readonly ISubscriptionRepository _repository = repository;
    private readonly IUserService _userService = userService;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IMapper _mapper = mapper;

    public async Task<List<SubscriptionContract>> Get(string? id = null, string? userId = null)
    {
        return _mapper.Map<List<SubscriptionContract>>(await _repository.Get(id, userId));
    }

    public async Task<SubscriptionContract?> Create(CreateSubscriptionContract create, string token)
    {
        var userId = _authenticationService.GetId(token);
        if (userId == null) return null;
        var subscription = _mapper.Map<Subscription>(create);
        subscription.UserId = userId;
        subscription = await _repository.Create(subscription);
        if (subscription == null) return null;
        await _userService.Subscribe(userId, subscription.Id);
        return _mapper.Map<SubscriptionContract>(subscription);
    }

    public async Task<SubscriptionContract?> Delete(string id)
    {
        var subscription = await _repository.Delete(id);
        if (subscription == null) return null;
        await _userService.Unsubscribe(subscription.UserId);
        return _mapper.Map<SubscriptionContract>(subscription);
    }
}
