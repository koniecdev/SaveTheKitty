using SaveTheKitty.API.Entities.Cats;
using SaveTheKitty.API.Entities.Users;

namespace SaveTheKitty.API.Databases.Interfaces;

public interface IMainDbContext
{
    public DbSet<ApplicationUser> ApplicationUsers { get; }
    public DbSet<Cat> Cats { get; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
