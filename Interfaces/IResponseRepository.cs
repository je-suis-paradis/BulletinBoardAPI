namespace BulletinBoardAPI.Interfaces;

public interface IResponseRepository
{
    Task<List<Models.PostResponse>> GetAllAsync();
    Task<Models.PostResponse?> GetByIdAsync(int id);
    Task<List<Models.PostResponse>> GetByPostIdAsync(int postId);
    Task<Models.PostResponse> CreateAsync(Models.PostResponse response, int userId);
    Task DeleteAsync(Models.PostResponse response);
}