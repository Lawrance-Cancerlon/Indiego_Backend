using System;
using AutoMapper;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;

namespace Indiego_Backend.Mappers;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<User, UserContract>().ForMember(p => p.Role, option => option.MapFrom(source => source.GetType().Name));
        CreateMap<CreateUserContract, User>();
        CreateMap<UpdateUserContract, User>().ForAllMembers(p => p.Condition((source, destination, member) => member != null));

        CreateMap<Admin, AdminContract>().ForMember(p => p.Role, option => option.MapFrom(source => source.GetType().Name));
        CreateMap<CreateAdminContract, Admin>();
        CreateMap<UpdateAdminContract, Admin>().ForAllMembers(p => p.Condition((source, destination, member) => member != null));

        CreateMap<Customer, CustomerContract>()
            .ForMember(p => p.Role, option => option.MapFrom(source => source.GetType().Name))
            .ForMember(p => p.IsSubscribed, option => option.MapFrom(source => source.SubscriptionId != null));
        CreateMap<CreateCustomerContract, Customer>();
        CreateMap<UpdateCustomerContract, Customer>().ForAllMembers(p => p.Condition((source, destination, member) => member != null));

        CreateMap<Developer, DeveloperContract>()
            .ForMember(p => p.Role, option => option.MapFrom(source => source.GetType().Name))
            .ForMember(p => p.IsSubscribed, option => option.MapFrom(source => source.SubscriptionId != null));
        CreateMap<UpdateDeveloperContract, Developer>().ForAllMembers(p => p.Condition((source, destination, member) => member != null));
    }
}
