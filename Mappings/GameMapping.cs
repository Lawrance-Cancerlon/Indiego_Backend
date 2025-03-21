using System;
using AutoMapper;
using Indiego_Backend.Contracts.Games;
using Indiego_Backend.Models.Games;

namespace Indiego_Backend.Mappings;

public class GameMapping : Profile
{
    public GameMapping()
    {
        CreateMap<Game, GameContract>();
        CreateMap<CreateGameContract, Game>();
        CreateMap<UpdateGameContract, Game>().ForAllMembers(p => p.Condition((source, destination, member) => member != null));

        CreateMap<Genre, GenreContract>();
        CreateMap<CreateGenreContract, Genre>();
    }
}
