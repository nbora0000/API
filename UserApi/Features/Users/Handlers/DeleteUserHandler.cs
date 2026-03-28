using MediatR;
using Microsoft.EntityFrameworkCore;
using UserApi.Data;
using UserApi.Features.Users.Commands;

namespace UserApi.Features.Users.Handlers;
public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly UserDbContext _context;

    public DeleteUserHandler(UserDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken ct)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (user == null) return false;

        // Remove associated tokens
        var tokens = _context.UserTokens.Where(t => t.UserId == request.Id);
        _context.UserTokens.RemoveRange(tokens);

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return true;
    }
}