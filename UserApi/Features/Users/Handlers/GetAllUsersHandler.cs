using MediatR;
using Microsoft.EntityFrameworkCore;
using UserApi.Data;
using UserApi.Features.Users.DTOs;
using UserApi.Features.Users.Queries;

namespace UserApi.Features.Users.Handlers;

public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    private readonly UserDbContext _context;

    public GetAllUsersHandler(UserDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken ct)
    {
        return await _context.Users
            .Select(x => new UserDto
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email
            })
            .ToListAsync();
    }
}