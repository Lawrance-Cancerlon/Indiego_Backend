using System;
using AutoMapper;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;
using Indiego_Backend.Repositories;

namespace Indiego_Backend.Services;

public interface ISubscriptionTypeService
{
    Task<List<SubscriptionTypeContract>> Get(string? id = null);
    Task<SubscriptionTypeContract?> Create(CreateSubscriptionTypeContract create);
    Task<SubscriptionTypeContract?> Update(string id, UpdateSubscriptionTypeContract update);
    Task<SubscriptionTypeContract?> Delete(string id);
}

public class SubscriptionTypeService(ISubscriptionTypeRepository repository, IMapper mapper) : ISubscriptionTypeService
{
    private readonly ISubscriptionTypeRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<List<SubscriptionTypeContract>> Get(string? id = null)
    {
        return _mapper.Map<List<SubscriptionTypeContract>>(await _repository.Get(id));
    }

    public async Task<SubscriptionTypeContract?> Create(CreateSubscriptionTypeContract create)
    {
        var subscriptionType = _mapper.Map<SubscriptionType>(create);
        return _mapper.Map<SubscriptionTypeContract>(await _repository.Create(subscriptionType));
    }

    public async Task<SubscriptionTypeContract?> Update(string id, UpdateSubscriptionTypeContract update)
    {
        var subscriptionType = (await _repository.Get(id)).FirstOrDefault();
        if (subscriptionType == null) return null;
        var updatedSubscriptionType = await _repository.Update(id, _mapper.Map(update, subscriptionType));
        return _mapper.Map<SubscriptionTypeContract>(updatedSubscriptionType);
    }

    public async Task<SubscriptionTypeContract?> Delete(string id)
    {
        var subscriptionType = await _repository.Delete(id);
        return _mapper.Map<SubscriptionTypeContract>(subscriptionType);
    }
}
