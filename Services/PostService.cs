using System;
using AutoMapper;
using Indiego_Backend.Contracts;
using Indiego_Backend.Models;
using Indiego_Backend.Repositories;

namespace Indiego_Backend.Services;

public interface IPostService
{
    Task<List<PostContract>> Get(string? id = null, string? userId = null);
    Task<List<PostContract>> GetLikes(string userId, IUserService userService);
    Task<PostContract?> Create(CreatePostContract create, string token, IUserService userService);
    Task<PostContract?> Update(string id, UpdatePostContract update);
    Task<PostContract?> Delete(string id, IUserService userService);
    Task AddLike(string postId, string token, IUserService userService);
    Task RemoveLike(string postId, string token, IUserService userService);
}

public class PostService(
    IPostRepository repository,
    IAuthenticationService authenticationService,
    IMapper mapper
) : IPostService
{
    private readonly IPostRepository _repository = repository;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IMapper _mapper = mapper;

    public async Task<List<PostContract>> Get(string? id = null, string? userId = null)
    {
        return _mapper.Map<List<PostContract>>(await _repository.Get(id, userId));
    }

    public async Task<List<PostContract>> GetLikes(string userId, IUserService userService)
    {
        IUserService _userService = userService;
        var user = (await _userService.Get<CustomerContract, Customer>(userId, null)).FirstOrDefault();
        if (user == null) return [];
        return _mapper.Map<List<PostContract>>(await _repository.GetLikes(user.Likes));
    }

    public async Task<PostContract?> Create(CreatePostContract create, string token, IUserService userService)
    {
        IUserService _userService = userService;

        var userId = _authenticationService.GetId(token);
        if (userId == null) return null;
        var post = _mapper.Map<Post>(create);
        post.UserId = userId;
        post = await _repository.Create(post);
        if (post == null) return null;
        await _userService.AddPost(userId, post.Id);
        return _mapper.Map<PostContract>(post);
    }

    public async Task<PostContract?> Update(string id, UpdatePostContract update)
    {
        var post = (await _repository.Get(id)).FirstOrDefault();
        if (post == null) return null;
        var updatedPost = await _repository.Update(id, _mapper.Map(update, post));
        return _mapper.Map<PostContract>(updatedPost);
    }

    public async Task<PostContract?> Delete(string id, IUserService userService)
    {
        IUserService _userService = userService;

        var post = await _repository.Delete(id);
        if (post == null) return null;
        await _userService.RemovePost(post.UserId, id);
        return _mapper.Map<PostContract>(post);
    }

    public async Task AddLike(string postId, string token, IUserService userService)
    {
        IUserService _userService = userService;

        var userId = _authenticationService.GetId(token);
        if (userId == null) return;

        var post = (await _repository.Get(postId)).FirstOrDefault();
        if (post == null) return;

        if (!post.Likes.Contains(userId)) post.Likes.Add(userId);
        await _userService.AddLike(userId, postId);
        await _repository.Update(postId, post);
    }

    public async Task RemoveLike(string postId, string token, IUserService userService)
    {
        IUserService _userService = userService;

        var userId = _authenticationService.GetId(token);
        if (userId == null) return;

        var post = (await _repository.Get(postId)).FirstOrDefault();
        if (post == null) return;

        post.Likes.Remove(userId);
        await _userService.RemoveLike(userId, postId);
        await _repository.Update(postId, post);
    }
}
