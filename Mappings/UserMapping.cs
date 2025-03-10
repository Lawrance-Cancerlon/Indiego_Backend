using System;
using AutoMapper;
using Indiego_Backend.Contracts.Users;
using Indiego_Backend.Models.Users;

namespace Indiego_Backend.Mappings;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<User, UserContract>().ForMember(p => p.Role, option => option.MapFrom(source => source.GetType().Name));
        CreateMap<CreateUserContract, User>();
        CreateMap<UpdateUserContract, User>().ForAllMembers(p => p.Condition((source, destination, member) => member != null));

        CreateMap<Admin, AdminContract>().ForMember(p => p.Role, option => option.MapFrom(source => source.GetType().Name));
        CreateMap<CreateAdminContract, Admin>();
        CreateMap<UpdateAdminContract, Admin>().IncludeBase<UpdateUserContract, User>();

        CreateMap<Customer, CustomerContract>().ForMember(p => p.Role, option => option.MapFrom(source => source.GetType().Name));
        CreateMap<CreateCustomerContract, Customer>();
        CreateMap<UpdateCustomerContract, Customer>().IncludeBase<UpdateUserContract, User>();

        CreateMap<Developer, DeveloperContract>().ForMember(p => p.Role, option => option.MapFrom(source => source.GetType().Name));
        CreateMap<UpdateDeveloperContract, Developer>().IncludeBase<UpdateUserContract, User>();
    }
}
