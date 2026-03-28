using MediatR;

namespace UserApi.Features.Users.Commands;

public class UpdateUserRoleCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string NewRole { get; set; }
}
