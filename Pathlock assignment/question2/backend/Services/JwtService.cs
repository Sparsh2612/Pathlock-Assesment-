using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace PathlockProjects.Services;

public interface IJwtService
{
    string GenerateToken(Guid userId, string username);
}

public class JwtService(IConfiguration config) : IJwtService
{
    public string GenerateToken(Guid userId, string username)
    {
        var section = config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(section["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(int.Parse(section["ExpiresMinutes"]!));
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username)
        };
        var token = new JwtSecurityToken(
            issuer: section["Issuer"],
            audience: section["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}


