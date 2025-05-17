using System;
using AutoMapper;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;
using Indiego_Backend.Repositories;
using Indiego_Backend.Utilities;

namespace Indiego_Backend.Mappers;

public class SubscriptionExpiryResolver(ISubscriptionTypeRepository subscriptionTypeRepository) : IValueResolver<CreateSubscriptionContract, Subscription, string>
{
    private readonly ISubscriptionTypeRepository _subscriptionTypeRepository = subscriptionTypeRepository;

    public string Resolve(CreateSubscriptionContract source, Subscription destination, string destMember, ResolutionContext context)
    {
        var subscriptionType = _subscriptionTypeRepository.Get(source.SubscriptionTypeId).Result.FirstOrDefault();
        if (subscriptionType == null) return string.Empty;
        return DatetimeUtility.ToUnixTimestampString(DateTime.Now.AddDays(subscriptionType.Duration));
    }
}

public class SubscriptionMapper : Profile
{
    public SubscriptionMapper()
    {
        CreateMap<Subscription, SubscriptionContract>();
        CreateMap<CreateSubscriptionContract, Subscription>()
            .ForMember(p => p.Expire, option => option.MapFrom<SubscriptionExpiryResolver>());
    }
}
