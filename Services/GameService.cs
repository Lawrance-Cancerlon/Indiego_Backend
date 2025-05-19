using System;
using AutoMapper;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;
using Indiego_Backend.Repositories;

namespace Indiego_Backend.Services;

public interface IGameService
{
    Task<List<GameContract>> Get(string? id = null, string? developerId = null, string? genreId = null);
    Task<GameContract?> Create(CreateGameContract create, string token, IGenreService genreService, IUserService userService);
    Task<GameContract?> Update(string id, UpdateGameContract update);
    Task<GameContract?> Delete(string id, IGenreService genreService, IReviewService reviewService, IUserService userService);
    Task Download(string gameId, string token, IUserService userService);
    Task AddReview(string gameId, string reviewId);
    Task RemoveReview(string gameId, string reviewId);
    Task RemoveGenre(string gameId, string genreId);
}

public class GameService(
    IGameRepository repository,
    IAuthenticationService authenticationService,
    IMapper mapper
) : IGameService
{
    private readonly IGameRepository _repository = repository;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IMapper _mapper = mapper;

    public async Task<List<GameContract>> Get(string? id = null, string? developerId = null, string? genreId = null)
    {
        return _mapper.Map<List<GameContract>>(await _repository.Get(id, developerId, genreId));
    }

    public async Task<GameContract?> Create(CreateGameContract create, string token, IGenreService genreService, IUserService userService)
    {
        IGenreService _genreService = genreService;
        IUserService _userService = userService;
        var userId = _authenticationService.GetId(token);
        if (userId == null) return null;
        var game = _mapper.Map<Game>(create);
        game.UserId = userId;
        game = await _repository.Create(game);
        if (game == null) return null;
        await _userService.AddGame(userId, game.Id);
        foreach (var genre in game.GenreIds) await _genreService.AddGame(genre, game.Id);
        return _mapper.Map<GameContract>(game);
    }

    public async Task<GameContract?> Update(string id, UpdateGameContract update)
    {
        var game = (await _repository.Get(id)).FirstOrDefault();
        if (game == null) return null;
        var updatedGame = await _repository.Update(id, _mapper.Map(update, game));
        return _mapper.Map<GameContract>(updatedGame);
    }

    public async Task<GameContract?> Delete(string id, IGenreService genreService, IReviewService reviewService, IUserService userService)
    {
        IGenreService _genreService = genreService;
        IReviewService _reviewService = reviewService;
        IUserService _userService = userService;

        var game = await _repository.Delete(id);
        if (game == null) return null;

        foreach (var genre in game.GenreIds) await _genreService.RemoveGame(genre, id);
        foreach (var review in game.ReviewIds) await _reviewService.Delete(review, _userService, gameService: this);
        await _userService.RemoveGame(game.UserId, id);
        return _mapper.Map<GameContract>(game);
    }

    public async Task Download(string gameId, string token, IUserService userService)
    {
        IUserService _userService = userService;

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
        await _userService.Download(userId, game.Id);
        await _repository.Update(gameId, game);
    }

    public async Task AddReview(string gameId, string reviewId)
    {
        var game = (await _repository.Get(gameId)).FirstOrDefault();
        if (game == null) return;

        if (!game.ReviewIds.Contains(reviewId)) game.ReviewIds.Add(reviewId);
        await _repository.Update(gameId, game);
    }

    public async Task RemoveReview(string gameId, string reviewId)
    {
        var game = (await _repository.Get(gameId)).FirstOrDefault();
        if (game == null) return;

        game.ReviewIds.Remove(reviewId);
        await _repository.Update(gameId, game);
    }

    public async Task RemoveGenre(string gameId, string genreId)
    {
        var game = (await _repository.Get(gameId)).FirstOrDefault();
        if (game == null) return;

        game.GenreIds.Remove(genreId);
        await _repository.Update(gameId, game);
    }
}
