using BulletinBoardAPI.Models;

namespace BulletinBoardAPI.Interfaces;

public interface IAuthRepository
{
    Task<bool> UserExistsAsync(string email);
    Task<User> RegisterAsync(DTOs.RegisterDto dto);
    Task<User?> LoginAsync(DTOs.LoginDto dto);
}