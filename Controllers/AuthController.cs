using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Dtos;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;  // ✅ Fix variable name
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto loginDto)
    {
        if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
        {
            return BadRequest(new { message = "Username and Password are required." });
        }

        var user = _context.Users.FirstOrDefault(u => u.Username == loginDto.Username);
        if (user == null || user.PasswordHash != loginDto.Password)
        {
            return Unauthorized(new { message = "Invalid credentials." });
        }

        var token = GenerateJwtToken(user);
        return Ok(new { token });
    }

    private string GenerateJwtToken(User user)
    {
        // ✅ Retrieve values safely
        string key = _config["Jwt:Key"];
        string issuer = _config["Jwt:Issuer"];
        string audience = _config["Jwt:Audience"];

        // ✅ Validate JWT Key
        if (string.IsNullOrEmpty(key) || key.Length < 16)
        {
            throw new ArgumentException("JWT Key must be at least 16 characters long in appsettings.json");
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
