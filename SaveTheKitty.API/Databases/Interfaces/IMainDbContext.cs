using SaveTheKitty.API.Entities.Users;

namespace SaveTheKitty.API.Databases.Interfaces;

public interface IMainDbContext
{
    public DbSet<ApplicationUser> ApplicationUsers { get; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
