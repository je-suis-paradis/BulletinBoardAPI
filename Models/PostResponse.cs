using BulletinBoardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BulletinBoardAPI.Models;

public class PostResponse
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int PostId { get; set; }
    public int AuthorId { get; set; }

    public Post? Post { get; set; } = null!;
    public User? Author { get; set; } = null!;
}