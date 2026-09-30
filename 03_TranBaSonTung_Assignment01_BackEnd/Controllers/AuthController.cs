using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackEnd.BusinessObjects;
using BackEnd.DTOs;
using BackEnd.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BackEnd.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ISystemAccountRepository _repository;
    private readonly IConfiguration _configuration;

    public AuthController(ISystemAccountRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        // Default admin account comes from appsettings.json
        var adminEmail = _configuration["AdminAccount:Email"];
        var adminPassword = _configuration["AdminAccount:Password"];
        if (string.Equals(request.Email, adminEmail, StringComparison.OrdinalIgnoreCase) && request.Password == adminPassword)
        {
            return Ok(BuildResponse(0, "Administrator", adminEmail!, AccountRoles.AdminName));
        }

        var account = _repository.Login(request.Email, request.Password);
        if (account == null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        if (account.AccountRole != AccountRoles.Staff)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new { message = "You do not have permission to access this system." });
        }

        return Ok(BuildResponse(account.AccountID, account.AccountName ?? "", account.AccountEmail ?? "", AccountRoles.StaffName));
    }

    private LoginResponse BuildResponse(short id, string name, string email, string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue("Jwt:ExpireMinutes", 120)),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            AccountID = id,
            AccountName = name,
            AccountEmail = email,
            Role = role
        };
    }
}
