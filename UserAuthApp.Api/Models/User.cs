using System.ComponentModel.DataAnnotations;

namespace UserAuthApp.Api.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    public int? Age { get; set; }

    [StringLength(50)]
    public string? Country { get; set; }

    [StringLength(50)]
    public string? City { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    [Required]
    public string PasswordHash { get; set; } = string.Empty;
} 