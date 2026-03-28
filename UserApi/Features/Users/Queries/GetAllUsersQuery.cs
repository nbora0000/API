using MediatR;
using System.Collections.Generic;
using UserApi.Features.Users.DTOs;

namespace UserApi.Features.Users.Queries;

public class GetAllUsersQuery : IRequest<List<UserDto>>
{
}