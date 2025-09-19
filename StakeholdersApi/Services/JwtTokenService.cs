using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StakeholdersApi.Store.Models;

namespace StakeholdersApi.Services;

public interface IJwtTokenService { string Generate(User user); }

public class JwtTokenService : IJwtTokenService
{
    private readonly string _secret;
    public JwtTokenService(string secret) => _secret = secret;
    public string Generate(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddHours(24), signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
