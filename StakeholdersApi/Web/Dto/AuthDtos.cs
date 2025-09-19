using System.ComponentModel.DataAnnotations;
namespace StakeholdersApi.Web.Dto;
public class RegisterRequest
{
    [Required] public string Username { get; set; } = default!;
    [Required, EmailAddress] public string Email { get; set; } = default!;
    [Required, MinLength(6)] public string Password { get; set; } = default!;
    [Required] public string Role { get; set; } = "TOURIST"; // GUIDE|TOURIST
}
public class LoginRequest
{
    public string? UsernameOrEmail { get; set; }
    [Required] public string Password { get; set; } = default!;
}
public class AuthResponse { public string Token { get; set; } = default!; }
