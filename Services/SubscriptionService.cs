using System;
using AutoMapper;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;
using Indiego_Backend.Repositories;

namespace Indiego_Backend.Services;

public interface ISubscriptionService
{
    Task<List<SubscriptionContract>> Get(string? id = null, string? userId = null);
    Task<SubscriptionContract?> Create(CreateSubscriptionContract create, string token, IUserService userService);
    Task<SubscriptionContract?> Delete(string id, IUserService userService);
    Task AddDownload(string token, string gameId, IUserService userService, IGameService gameService);
}

public class SubscriptionService(ISubscriptionRepository repository, IAuthenticationService authenticationService, IMapper mapper) : ISubscriptionService
{
    private readonly ISubscriptionRepository _repository = repository;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IMapper _mapper = mapper;

    public async Task<List<SubscriptionContract>> Get(string? id = null, string? userId = null)
    {
        return _mapper.Map<List<SubscriptionContract>>(await _repository.Get(id, userId));
    }

    public async Task<SubscriptionContract?> Create(CreateSubscriptionContract create, string token, IUserService userService)
    {
        IUserService _userService = userService;

        var userId = _authenticationService.GetId(token);
        if (userId == null) return null;
        var subscription = _mapper.Map<Subscription>(create);
        subscription.UserId = userId;
        subscription = await _repository.Create(subscription);
        if (subscription == null) return null;
        await _userService.Subscribe(userId, subscription.Id);
        return _mapper.Map<SubscriptionContract>(subscription);
    }

    public async Task<SubscriptionContract?> Delete(string id, IUserService userService)
    {
        IUserService _userService = userService;

        var subscription = await _repository.Delete(id);
        if (subscription == null) return null;
        await _userService.Unsubscribe(subscription.UserId);
        return _mapper.Map<SubscriptionContract>(subscription);
    }

    public async Task AddDownload(string token, string gameId, IUserService userService, IGameService gameService)
    {
        IUserService _userService = userService;
        IGameService _gameService = gameService;

        var userId = _authenticationService.GetId(token);
        if (userId == null) return;

        var user = (await _userService.Get<CustomerContract, Customer>(userId)).FirstOrDefault();
        if (user == null) return;

        var subscription = (await _repository.Get(user.SubscriptionId)).FirstOrDefault();
        if (subscription == null) return;
        if (subscription.Download < 10)
        {
            var game = (await _gameService.Get(gameId)).FirstOrDefault();
            if (game == null) return;

            await _userService.AddBalance(game.UserId, 1000);

            subscription.Download++;
        }
        await _repository.Update(user.SubscriptionId!, subscription);
    }
}
