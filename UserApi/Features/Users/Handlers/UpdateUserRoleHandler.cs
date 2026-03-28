using MediatR;
using Microsoft.EntityFrameworkCore;
using UserApi.Data;
using UserApi.Features.Users.Commands;

namespace UserApi.Features.Users.Handlers;

public class UpdateUserRoleHandler : IRequestHandler<UpdateUserRoleCommand, bool>
{
    private readonly UserDbContext _context;

    public UpdateUserRoleHandler(UserDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateUserRoleCommand request, CancellationToken ct)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (user == null) return false;

        user.Role = request.NewRole;
        user.UpdatedAt = DateTime.UtcNow;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(ct);

        return true;
    }
}
