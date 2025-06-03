using Microsoft.EntityFrameworkCore;
using UserAuthApp.Api.Data;
using UserAuthApp.Api.Models;
using UserAuthApp.Api.Models.DTOs;

namespace UserAuthApp.Api.Services;

public interface IAuthService
{
    Task<User> RegisterAsync(RegisterDto registerDto);
    Task<User> LoginAsync(LoginDto loginDto);
    Task<IEnumerable<User>> GetAllUsersAsync();
}

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> RegisterAsync(RegisterDto registerDto)
    {
        if (string.IsNullOrWhiteSpace(registerDto.Email))
        {
            throw new InvalidOperationException("Email is required");
        }

        var emailExists = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == registerDto.Email.ToLower());

        if (emailExists)
        {
            throw new InvalidOperationException("Email already exists");
        }

        var user = new User
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            Age = registerDto.Age
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> LoginAsync(LoginDto loginDto)
    {
        if (string.IsNullOrWhiteSpace(loginDto.Email))
        {
            throw new InvalidOperationException("Email is required");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower());

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new InvalidOperationException("Invalid email or password");
        }

        return user;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }
} 