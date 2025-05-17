using System;
using AutoMapper;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;

namespace Indiego_Backend.Mappers;

public class SubscriptionTypeMapper : Profile
{
    public SubscriptionTypeMapper()
    {
        CreateMap<SubscriptionType, SubscriptionTypeContract>();
        CreateMap<CreateSubscriptionTypeContract, SubscriptionType>();
        CreateMap<UpdateSubscriptionTypeContract, SubscriptionType>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
