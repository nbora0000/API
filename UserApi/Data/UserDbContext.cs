using Microsoft.EntityFrameworkCore;
using UserApi.Entities;

namespace UserApi.Data;
public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }
}
