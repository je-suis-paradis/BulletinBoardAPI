using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BulletinBoardAPI.Models;
using BulletinBoardAPI.Data;
using Microsoft.AspNetCore.Authorization;

namespace BulletinBoardAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResponsesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ResponsesController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostResponse>>> GetResponses()
    {
        return await _context.PostResponses
            .Include(r => r.Author)
            .Include(r => r.Post)
            .ToListAsync();
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<PostResponse>> GetResponse(int id)
    {
        var response = await _context.PostResponses
            .Include(r => r.Author)
            .Include(r => r.Post)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (response == null)
        {
            return NotFound();
        }

        return response;
    }


    [HttpGet("post/{postId}")]
    public async Task<ActionResult<IEnumerable<PostResponse>>> GetResponsesByPost(int postId)
    {
        return await _context.PostResponses
            .Include(r => r.Author)
            .Where(r => r.PostId == postId)
            .ToListAsync();
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<PostResponse>> CreateResponse(PostResponse response)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim.Value);

        var post = await _context.Posts.FindAsync(response.PostId);
        if (post == null)
        {
            return BadRequest("Post not found");
        }

        response.AuthorId = userId;
        response.Author = null!;
        response.Post = null!;
        response.CreatedAt = DateTime.UtcNow;

        _context.PostResponses.Add(response);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetResponse), new { id = response.Id }, response);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateResponse(int id, PostResponse response)
    {
        if (id != response.Id)
        {
            return BadRequest();
        }

        _context.Entry(response).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ResponseExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteResponse(int id)
    {
        var response = await _context.PostResponses.FindAsync(id);
        if (response == null)
        {
            return NotFound();
        }

        _context.PostResponses.Remove(response);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ResponseExists(int id)
    {
        return _context.PostResponses.Any(e => e.Id == id);
    }
}