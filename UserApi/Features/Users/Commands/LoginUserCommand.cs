using MediatR;

namespace UserApi.Features.Users.Commands;

public class LoginUserCommand : IRequest<string>
{
    public string Email { get; set; }
    public string Password { get; set; }
}
