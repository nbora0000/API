using BCrypt.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserApi.Data;
using UserApi.Features.Users.Commands;

namespace UserApi.Features.Users.Handlers;
public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, bool>
{
    private readonly UserDbContext _context;

    public ResetPasswordHandler(UserDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null) return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        await _context.SaveChangesAsync();

        return true;
    }
}
