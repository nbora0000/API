using MediatR;
using UserApi.Features.Users.DTOs;

namespace UserApi.Features.Users.Queries;
public class GetUserByIdQuery : IRequest<UserDto>
{
    public Guid Id { get; set; }
}