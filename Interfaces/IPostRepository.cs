namespace BulletinBoardAPI.Interfaces;

public interface IPostRepository
{
    Task<List<Models.Post>> GetAllAsync();
    Task<Models.Post?> GetByIdAsync(int id);
    Task<Models.Post> CreateAsync(Models.Post post);
    Task DeleteAsync(Models.Post post);
}