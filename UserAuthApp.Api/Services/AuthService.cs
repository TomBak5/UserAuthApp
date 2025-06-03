using Microsoft.EntityFrameworkCore;
using UserAuthApp.Api.Data;
using UserAuthApp.Api.Models;
using UserAuthApp.Api.Models.DTOs;

namespace UserAuthApp.Api.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(RegisterDto registerDto);
    Task<User?> LoginAsync(LoginDto loginDto);
    Task<List<User>> GetAllUsersAsync();
}

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> RegisterAsync(RegisterDto registerDto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
        {
            return null;
        }

        var user = new User
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            PhoneNumber = registerDto.PhoneNumber,
            Age = registerDto.Age,
            Country = registerDto.Country,
            City = registerDto.City,
            Address = registerDto.Address,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User?> LoginAsync(LoginDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user == null)
        {
            return null;
        }

        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            return null;
        }

        return user;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }
} 