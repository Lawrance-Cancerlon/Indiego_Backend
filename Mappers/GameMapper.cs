using System;
using AutoMapper;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;

namespace Indiego_Backend.Mappers;

public class GameMapper : Profile
{
    public GameMapper()
    {
        CreateMap<Game, GameContract>()
            .ForMember(p => p.Downloads, option => option.MapFrom(source => source.Downloads.Count));
        CreateMap<CreateGameContract, Game>();
        CreateMap<UpdateGameContract, Game>()
            .ForAllMembers(p => p.Condition((source, destination, member) => member != null));
    }
}
