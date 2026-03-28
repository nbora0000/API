using MediatR;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using UserApi.Features.Users.Commands;
using UserApi.Data;
using UserApi.Entities;

namespace UserApi.Features.Users.Handlers;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, string>
{
    private readonly UserDbContext _context;
    private readonly IConfiguration _config;

    public LoginUserHandler(UserDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<string> Handle(LoginUserCommand request, CancellationToken ct)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new Exception("Invalid credentials");

        var (token, expiry) = GenerateJwt(user);

        _context.UserTokens.Add(new UserToken
        {
            UserId = user.Id,
            Token = token,
            ExpiryDate = expiry
        });

        await _context.SaveChangesAsync();

        return token;
    }

    private (string, DateTime) GenerateJwt(User user)
    {
        var jwtKey = _config["Jwt:Key"] ?? "SUPER_SECRET_KEY_1234567890123456_EXTENDED_KEY_48";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiry = DateTime.UtcNow.AddMinutes(30);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expiry,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiry);
    }
}
