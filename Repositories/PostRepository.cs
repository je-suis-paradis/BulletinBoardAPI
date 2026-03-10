using BulletinBoardAPI.Data;
using BulletinBoardAPI.Models;
using BulletinBoardAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BulletinBoardAPI.Repositories;

public class PostRepository : IPostRepository
{
    private readonly AppDbContext _context;

    public PostRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Post>> GetAllAsync()
    {
        return await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Responses)
            .ToListAsync();
    }

    public async Task<Post?> GetByIdAsync(int id)
    {
        return await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Responses)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Post> CreateAsync(Post post)
    {
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        return post;
    }

    public async Task DeleteAsync(Post post)
    {
        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
    }
}