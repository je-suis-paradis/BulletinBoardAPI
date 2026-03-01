using Microsoft.EntityFrameworkCore;
using BulletinBoardAPI.Models;

namespace BulletinBoardAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostResponse> PostResponses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<PostResponse>()
            .HasOne(r => r.Author)
            .WithMany(u => u.Responses)
            .HasForeignKey(r => r.AuthorId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PostResponse>()
            .HasOne(r => r.Post)
            .WithMany(p => p.Responses)
            .HasForeignKey(r => r.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}