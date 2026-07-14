using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace BackEnd_Synthetis.Security;

public class JwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(string usuario)
    {
        var jwtKey =
            _configuration["Jwt:Key"];

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!)
            );

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, usuario)
        };

        var token =
            new JwtSecurityToken(
                claims: claims,
                expires:
                    DateTime.UtcNow.AddHours(6),
                signingCredentials: credentials
            );

        return
            new JwtSecurityTokenHandler()
                .WriteToken(token);
    }
}