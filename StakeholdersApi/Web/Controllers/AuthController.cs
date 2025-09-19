using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using StakeholdersApi.Services;
using StakeholdersApi.Store;
using StakeholdersApi.Store.Models;
using StakeholdersApi.Web.Dto;

namespace StakeholdersApi.Web.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AppDb _db; private readonly IJwtTokenService _jwt;
    public AuthController(AppDb db, IJwtTokenService jwt) { _db = db; _jwt = jwt; }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (!Enum.TryParse<Role>(req.Role, true, out var role) || role == Role.ADMIN)
            return BadRequest("Role must be GUIDE or TOURIST.");
        var exists = await _db.Users.Find(u => u.Username == req.Username || u.Email == req.Email).AnyAsync();
        if (exists) return Conflict("Username or email already taken.");
        var user = new User
        {
            Username = req.Username,
            Email = req.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role = role
        };
        await _db.Users.InsertOneAsync(user);
        return Created($"/users/{user.Id}", new { user.Id, user.Username, user.Email, user.Role });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.UsernameOrEmail)) return BadRequest("Provide usernameOrEmail.");
        var user = await _db.Users.Find(u => u.Username == req.UsernameOrEmail || u.Email == req.UsernameOrEmail).FirstOrDefaultAsync();
        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash)) return Unauthorized("Invalid credentials.");
        if (user.Blocked) return Forbid();
        return Ok(new AuthResponse { Token = _jwt.Generate(user) });
    }
}
