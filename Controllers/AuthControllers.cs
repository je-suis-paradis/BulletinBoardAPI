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
    public async Task<ActionResult<AuthResponseDTO>> Register(RegisterDTO dto)
    {
        // Check if email already exists
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
        {
            return BadRequest("Email already registered");
        }

        // Check if handle already exists
        if (await _context.Users.AnyAsync(u => u.Handle == dto.Handle))
        {
            return BadRequest("Handle already taken");
        }

        // Create new user
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Handle = dto.Handle,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Generate token
        var token = _jwtService.GenerateToken(user);

        return Ok(new AuthResponseDTO
        {
            Token = token,
            Email = user.Email,
            Handle = user.Handle,
            Name = user.Name
        });
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDTO>> Login(LoginDTO dto)
    {
        // Find user by email
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
        {
            return Unauthorized("Invalid email or password");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid email or password");
        }

        // Generate token
        var token = _jwtService.GenerateToken(user);

        return Ok(new AuthResponseDTO
        {
            Token = token,
            Email = user.Email,
            Handle = user.Handle,
            Name = user.Name
        });
    }
}