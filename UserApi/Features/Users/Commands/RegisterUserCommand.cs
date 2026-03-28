using MediatR; 
using UserApi.Features.Users.DTOs;
namespace UserApi.Features.Users.Commands;

public class RegisterUserCommand : IRequest<UserDto>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; } = "User";
}