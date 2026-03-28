using MediatR;
using Microsoft.EntityFrameworkCore;
using UserApi.Data;
using UserApi.Features.Users.DTOs;
using UserApi.Features.Users.Queries;

namespace UserApi.Features.Users.Handlers;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly UserDbContext _context;

    public GetUserByIdHandler(UserDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }
}