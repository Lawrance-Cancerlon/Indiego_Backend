using System;
using AutoMapper;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;
using Indiego_Backend.Repositories;

namespace Indiego_Backend.Services;

public interface IGenreService
{
    Task<List<GenreContract>> Get(string? id = null);
    Task<GenreContract?> Create(CreateGenreContract create);
    Task<GenreContract?> Delete(string id);
}

public class GenreService(IGenreRepository repository, IMapper mapper) : IGenreService
{
    private readonly IGenreRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<List<GenreContract>> Get(string? id = null)
    {
        return _mapper.Map<List<GenreContract>>(await _repository.Get(id));
    }

    public async Task<GenreContract?> Create(CreateGenreContract create)
    {
        var genre = _mapper.Map<Genre>(create);
        return _mapper.Map<GenreContract>(await _repository.Create(genre));
    }

    public async Task<GenreContract?> Delete(string id)
    {
        var genre = await _repository.Delete(id);
        return _mapper.Map<GenreContract>(genre);
    }
}
