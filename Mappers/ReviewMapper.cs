using System;
using AutoMapper;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;
using Indiego_Backend.Utilities;

namespace Indiego_Backend.Mappers;

public class ReviewMapper : Profile
{
    public ReviewMapper()
    {
        CreateMap<Review, ReviewContract>();
        CreateMap<CreateReviewContract, Review>()
            .ForMember(p => p.CreatedAt, option => option.MapFrom(source => DatetimeUtility.ToUnixTimestampString(DateTime.Now)));
        CreateMap<UpdateReviewContract, Review>()
            .ForAllMembers(p => p.Condition((source, destination, member) => member != null));
    }
}
