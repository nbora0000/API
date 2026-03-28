using MediatR;
using BCrypt.Net;
using UserApi.Data;
using UserApi.Entities;
using UserApi.Features.Users.Commands;
using UserApi.Features.Users.DTOs;

namespace UserApi.Features.Users.Handlers;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, UserDto>
{
    private readonly UserDbContext _context;

    public RegisterUserHandler(UserDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Role = request.Role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(ct);

        return new UserDto { Id = user.Id, Name = user.Name, Email = user.Email };
    }
}