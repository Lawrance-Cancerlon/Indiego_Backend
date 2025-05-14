using System;
using AutoMapper;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;

namespace Indiego_Backend.Mappers;

public class GenreMapper : Profile
{
    public GenreMapper()
    {
        CreateMap<Genre, GenreContract>();
        CreateMap<CreateGenreContract, Genre>();
    }
}
