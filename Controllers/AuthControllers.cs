using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BulletinBoardAPI.Data;
using BulletinBoardAPI.Models;
using BulletinBoardAPI.DTOs;
using BulletinBoardAPI.Services;


namespace BulletinBoardAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
        {
            return BadRequest("Email already registered");
        }

        if (await _context.Users.AnyAsync(u => u.Handle == dto.Handle))
        {
            return BadRequest("Handle already taken");
        }

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Handle = dto.Handle,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            Handle = user.Handle,
            Username = user.Username
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
        {
            return Unauthorized("Invalid email or password");
        }

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid email or password");
        }

        var token = _jwtService.GenerateToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            Handle = user.Handle,
            Username = user.Username
        });
    }
}