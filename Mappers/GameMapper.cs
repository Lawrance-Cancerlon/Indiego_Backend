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
            .ForAllMembers(p => p.Condition((source, destination, sourceMember, destinationMember, context) => {
                if (sourceMember == null) return false;
                if (sourceMember is string str && string.IsNullOrEmpty(str)) return false;
                if (sourceMember is List<string> list && list.Count == 0) return false;
                return true;
            }));
    }
}
