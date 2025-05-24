using System;
using AutoMapper;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;

namespace Indiego_Backend.Mappers;

public class NullableBooleanResolver : IMemberValueResolver<object, object, bool?, bool>
{
    public bool Resolve(object source, object destination, bool? sourceMember, bool destinationMember, ResolutionContext context)
    {
        return sourceMember ?? destinationMember;
    }
}

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<User, UserContract>()
            .ForMember(p => p.Role, option => option.MapFrom(source => source.GetType().Name));
        CreateMap<CreateUserContract, User>()
            .ForMember(p => p.Password, option => option.MapFrom(source => BCrypt.Net.BCrypt.HashPassword(source.Password)));
        CreateMap<UpdateUserContract, User>()
            .ForMember(p => p.Password, option => option.MapFrom((src, dest) =>
                !string.IsNullOrEmpty(src.Password) ? BCrypt.Net.BCrypt.HashPassword(src.Password) : dest.Password))
            .ForAllMembers(p => p.Condition((source, destination, member) => member != null));

        CreateMap<Admin, AdminContract>()
            .ForMember(p => p.Role, option => option.MapFrom(source => source.GetType().Name));
        CreateMap<CreateAdminContract, Admin>()
            .ForMember(p => p.Password, option => option.MapFrom(source => BCrypt.Net.BCrypt.HashPassword(source.Password)));
        CreateMap<UpdateAdminContract, Admin>()
            .ForMember(p => p.Password, option => option.MapFrom((src, dest) =>
                !string.IsNullOrEmpty(src.Password) ? BCrypt.Net.BCrypt.HashPassword(src.Password) : dest.Password))
            .ForMember(p => p.CanManageAdmins, opt =>
                opt.MapFrom<NullableBooleanResolver, bool?>(src => src.CanManageAdmins))
            .ForMember(p => p.CanManageGames, opt =>
                opt.MapFrom<NullableBooleanResolver, bool?>(src => src.CanManageGames))
            .ForMember(p => p.CanManagePosts, opt =>
                opt.MapFrom<NullableBooleanResolver, bool?>(src => src.CanManagePosts))
            .ForMember(p => p.CanManageReviews, opt =>
                opt.MapFrom<NullableBooleanResolver, bool?>(src => src.CanManageReviews))
            .ForMember(p => p.CanManageSubscriptions, opt =>
                opt.MapFrom<NullableBooleanResolver, bool?>(src => src.CanManageSubscriptions))
            .ForAllMembers(p => p.Condition((source, destination, member) => member != null));

        CreateMap<Customer, CustomerContract>()
            .ForMember(p => p.Role, option => option.MapFrom(source => source.GetType().Name))
            .ForMember(p => p.IsSubscribed, option => option.MapFrom(source => source.SubscriptionId != null));
        CreateMap<CreateCustomerContract, Customer>()
            .ForMember(p => p.Password, option => option.MapFrom(source => BCrypt.Net.BCrypt.HashPassword(source.Password)));
        CreateMap<UpdateCustomerContract, Customer>()
            .ForMember(p => p.Password, option => option.MapFrom((src, dest) =>
                !string.IsNullOrEmpty(src.Password) ? BCrypt.Net.BCrypt.HashPassword(src.Password) : dest.Password))
            .ForAllMembers(p => p.Condition((source, destination, member) => member != null));

        CreateMap<Developer, DeveloperContract>()
            .ForMember(p => p.Role, option => option.MapFrom(source => source.GetType().Name))
            .ForMember(p => p.IsSubscribed, option => option.MapFrom(source => source.SubscriptionId != null));
        CreateMap<UpdateDeveloperContract, Developer>()
            .ForMember(p => p.Password, option => option.MapFrom((src, dest) =>
                !string.IsNullOrEmpty(src.Password) ? BCrypt.Net.BCrypt.HashPassword(src.Password) : dest.Password))
            .ForAllMembers(p => p.Condition((source, destination, member) => member != null));
    }
}
