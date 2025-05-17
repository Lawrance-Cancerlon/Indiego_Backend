using System;
using AutoMapper;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;
using Indiego_Backend.Repositories;
using Indiego_Backend.Utilities;

namespace Indiego_Backend.Mappers;

public class GameRatingResolver(IReviewRepository reviewRepository) : IValueResolver<Game, GameContract, decimal>
{
    private readonly IReviewRepository _reviewRepository = reviewRepository;

    public decimal Resolve(Game source, GameContract destination, decimal destMember, ResolutionContext context)
    {
        var reviews = _reviewRepository.Get(null, null, source.Id).Result;
        if (reviews.Count == 0) return 0;
        return (decimal)Math.Round(reviews.Average(r => r.Rating), 2);
    }
}

public class GameMapper : Profile
{
    public GameMapper()
    {
        CreateMap<Game, GameContract>()
            .ForMember(p => p.Downloads, option => option.MapFrom(source => source.Downloads.Count))
            .ForMember(p => p.Rating, option => option.MapFrom<GameRatingResolver>())
            .ForMember(p => p.RatingCount, option => option.MapFrom(source => source.ReviewIds.Count));
        CreateMap<CreateGameContract, Game>()
            .ForMember(p => p.CreatedAt, option => option.MapFrom(source => DatetimeUtility.ToUnixTimestampString(DateTime.Now)));
        CreateMap<UpdateGameContract, Game>()
            .ForAllMembers(p => p.Condition((source, destination, sourceMember, destinationMember, context) =>
            {
                if (sourceMember == null) return false;
                if (sourceMember is string str && string.IsNullOrEmpty(str)) return false;
                if (sourceMember is List<string> list && list.Count == 0) return false;
                return true;
            }));
    }
}
