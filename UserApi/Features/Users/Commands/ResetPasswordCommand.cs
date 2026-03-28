using MediatR;

namespace UserApi.Features.Users.Commands;

public class ResetPasswordCommand : IRequest<bool>
{
    public string Email { get; set; }
    public string NewPassword { get; set; }
}
