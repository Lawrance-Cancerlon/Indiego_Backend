using System;
using AutoMapper;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;
using Indiego_Backend.Utilities;

namespace Indiego_Backend.Mappers;

public class PostMapper : Profile
{
    public PostMapper()
    {
        CreateMap<Post, PostContract>()
            .ForMember(p => p.Likes, option => option.MapFrom(source => source.Likes.Count));
        CreateMap<CreatePostContract, Post>()
            .ForMember(p => p.CreatedAt, option => option.MapFrom(source => DatetimeUtility.ToUnixTimestampString(DateTime.Now)));
        CreateMap<UpdatePostContract, Post>()
            .ForAllMembers(p => p.Condition((source, destination, member) => member != null));
    }
}
