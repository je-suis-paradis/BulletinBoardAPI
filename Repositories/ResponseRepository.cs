using BulletinBoardAPI.Data;
using BulletinBoardAPI.Interfaces;
using BulletinBoardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BulletinBoardAPI.Repositories;

public class ResponseRepository : IResponseRepository
{
    private readonly AppDbContext _context;

    public ResponseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PostResponse>> GetAllAsync()
    {
        return await _context.PostResponses
            .Include(r => r.Author)
            .Include(r => r.Post)
            .ToListAsync();
    }

    public async Task<PostResponse?> GetByIdAsync(int id)
    {
        return await _context.PostResponses
            .Include(r => r.Author)
            .Include(r => r.Post)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<PostResponse>> GetByPostIdAsync(int postId)
    {
        return await _context.PostResponses
            .Include(r => r.Author)
            .Where(r => r.PostId == postId)
            .ToListAsync();
    }

    public async Task<PostResponse> CreateAsync(PostResponse response, int userId)
    {
        response.AuthorId = userId;
        response.Author = null!;
        response.Post = null!;
        response.CreatedAt = DateTime.UtcNow;
        _context.PostResponses.Add(response);
        await _context.SaveChangesAsync();
        return response;
    }

    public async Task DeleteAsync(PostResponse response)
    {
        _context.PostResponses.Remove(response);
        await _context.SaveChangesAsync();
    }
}