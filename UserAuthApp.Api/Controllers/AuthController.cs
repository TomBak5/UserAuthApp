using Microsoft.AspNetCore.Mvc;
using UserAuthApp.Api.Models.DTOs;
using UserAuthApp.Api.Services;

namespace UserAuthApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var user = await _authService.RegisterAsync(registerDto);
        if (user == null)
        {
            return BadRequest(new { message = "Email already exists" });
        }

        return Ok(new { message = "Registration successful" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var user = await _authService.LoginAsync(loginDto);
        if (user == null)
        {
            return BadRequest(new { message = "Invalid email or password" });
        }

        return Ok(new { message = "Login successful" });
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _authService.GetAllUsersAsync();
        return Ok(users);
    }
} 