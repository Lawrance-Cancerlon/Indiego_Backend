using System;
using AutoMapper;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;
using Indiego_Backend.Repositories;

namespace Indiego_Backend.Services;

public interface IGenreService
{
    Task<List<GenreContract>> Get(string? id = null, string? gameId = null);
    Task<GenreContract?> Create(CreateGenreContract create);
    Task<GenreContract?> Delete(string id);
    Task AddGame(string genreId, string gameId);
    Task RemoveGame(string genreId, string gameId);
}

public class GenreService(IGenreRepository repository, IGameService gameService, IMapper mapper) : IGenreService
{
    private readonly IGenreRepository _repository = repository;
    private readonly IGameService _gameService = gameService;
    private readonly IMapper _mapper = mapper;

    public async Task<List<GenreContract>> Get(string? id = null, string? gameId = null)
    {
        return _mapper.Map<List<GenreContract>>(await _repository.Get(id, gameId));
    }

    public async Task<GenreContract?> Create(CreateGenreContract create)
    {
        var genre = _mapper.Map<Genre>(create);
        return _mapper.Map<GenreContract>(await _repository.Create(genre));
    }

    public async Task<GenreContract?> Delete(string id)
    {
        var genre = await _repository.Delete(id);
        if (genre == null) return null;
        foreach (var game in genre.GameIds) await _gameService.RemoveGenre(game, id);
        return _mapper.Map<GenreContract>(genre);
    }

    public async Task AddGame(string genreId, string gameId)
    {
        var genre = (await _repository.Get(genreId)).FirstOrDefault();
        if (genre == null) return;

        if(!genre.GameIds.Contains(gameId)) genre.GameIds.Add(gameId);
        await _repository.Update(genreId, genre);
    }

    public async Task RemoveGame(string genreId, string gameId)
    {
        var genre = (await _repository.Get(genreId)).FirstOrDefault();
        if (genre == null) return;

        genre.GameIds.Remove(gameId);
        await _repository.Update(genreId, genre);
    }
}
