using System;
using AutoMapper;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;
using Indiego_Backend.Repositories;

namespace Indiego_Backend.Services;

public interface IGameService
{
    Task<List<GameContract>> Get(string? id = null, string? developerId = null, string? genreId = null);
    Task<GameContract?> Create(CreateGameContract create, string token);
    Task<GameContract?> Update(string id, UpdateGameContract update);
    Task<GameContract?> Delete(string id);
    Task Download(string token, string gameId);
    Task AddReview(string token, string reviewId);
    Task RemoveReview(string token, string reviewId);
}

public class GameService(IGameRepository repository, IAuthenticationService authenticationService, IMapper mapper) : IGameService
{
    private readonly IGameRepository _repository = repository;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IMapper _mapper = mapper;

    public async Task<List<GameContract>> Get(string? id = null, string? developerId = null, string? genreId = null)
    {
        return _mapper.Map<List<GameContract>>(await _repository.Get(id, developerId, genreId));
    }

    public async Task<GameContract?> Create(CreateGameContract create, string token)
    {
        var userId = _authenticationService.GetId(token);
        if (userId == null) return null;
        var game = _mapper.Map<Game>(create);
        game.UserId = userId;
        return _mapper.Map<GameContract>(await _repository.Create(game));
    }

    public async Task<GameContract?> Update(string id, UpdateGameContract update)
    {
        var game = (await _repository.Get(id)).FirstOrDefault();
        if (game == null) return null;
        var updatedGame = await _repository.Update(id, _mapper.Map(update, game));
        return _mapper.Map<GameContract>(updatedGame);
    }

    public async Task<GameContract?> Delete(string id)
    {
        var game = await _repository.Delete(id);
        return _mapper.Map<GameContract>(game);
    }

    public async Task Download(string token, string gameId)
    {
        var userId = _authenticationService.GetId(token);
        if (userId == null) return;

        var game = (await _repository.Get(gameId)).FirstOrDefault();
        if (game == null) return;

        var download = new Download
        {
            UserId = userId,
            GameId = game.Id
        };
        if (!game.Downloads.Any(d => d.UserId == userId && d.GameId == game.Id)) game.Downloads.Add(download);
        await _repository.Update(gameId, game);
    }
}
