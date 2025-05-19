using System;
using AutoMapper;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;
using Indiego_Backend.Repositories;

namespace Indiego_Backend.Services;

public interface IReviewService
{
    Task<List<ReviewContract>> Get(string? id = null, string? userId = null, string? gameId = null);
    Task<ReviewContract?> Create(CreateReviewContract create, string token, IUserService userService, IGameService gameService);
    Task<ReviewContract?> Update(string id, UpdateReviewContract update);
    Task<ReviewContract?> Delete(string id, IUserService userService, IGameService gameService);
}

public class ReviewService(IReviewRepository repository, IAuthenticationService authenticationService, IMapper mapper) : IReviewService
{
    private readonly IReviewRepository _repository = repository;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IMapper _mapper = mapper;

    public async Task<List<ReviewContract>> Get(string? id = null, string? userId = null, string? gameId = null)
    {
        return _mapper.Map<List<ReviewContract>>(await _repository.Get(id, userId, gameId));
    }

    public async Task<ReviewContract?> Create(CreateReviewContract create, string token, IUserService userService, IGameService gameService)
    {
        IUserService _userService = userService;
        IGameService _gameService = gameService;

        var userId = _authenticationService.GetId(token);
        if (userId == null) return null;
        var review = _mapper.Map<Review>(create);
        review.UserId = userId;
        review = await _repository.Create(review);
        if (review == null) return null;
        await _userService.AddReview(userId, review.Id);
        await _gameService.AddReview(review.GameId, review.Id);
        return _mapper.Map<ReviewContract>(review);
    }

    public async Task<ReviewContract?> Update(string id, UpdateReviewContract update)
    {
        var review = (await _repository.Get(id)).FirstOrDefault();
        if (review == null) return null;
        var updatedReview = await _repository.Update(id, _mapper.Map(update, review));
        return _mapper.Map<ReviewContract>(updatedReview);
    }

    public async Task<ReviewContract?> Delete(string id, IUserService userService, IGameService gameService)
    {
        IUserService _userService = userService;
        IGameService _gameService = gameService;

        var review = await _repository.Delete(id);
        if (review == null) return null;
        await _userService.RemoveReview(review.UserId, id);
        await _gameService.RemoveReview(review.GameId, id);
        return _mapper.Map<ReviewContract>(review);
    }
}
