using Microsoft.EntityFrameworkCore.ChangeTracking;
using SaveTheKitty.API.Common.Services.Interfaces;
using SaveTheKitty.API.Entities.Common;
using SaveTheKitty.API.Entities.Users;
using System.Reflection;

namespace SaveTheKitty.API.Databases;

internal class MainDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IMainDbContext
{
    private readonly IDateTime? _dateTime;

    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
    {
    }

    public MainDbContext(IDateTime dateTime, DbContextOptions<MainDbContext> options) : base(options)
    {
        _dateTime = dateTime;
    }

    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        //SeedHelper.Initialize(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        if (ChangeTracker.Entries<AuditableEntity>().Any())
        {
            foreach (EntityEntry<AuditableEntity> entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = "";
                        entry.Entity.Created = _dateTime!.Now;
                        entry.Entity.StatusId = 1;
                        break;
                    case EntityState.Modified:
                        entry.Entity.ModifiedBy = "";
                        entry.Entity.Modified = _dateTime!.Now;
                        break;
                    case EntityState.Deleted:
                        entry.Entity.ModifiedBy = "";
                        entry.Entity.Modified = _dateTime!.Now;
                        entry.Entity.Inactivated = _dateTime!.Now;
                        entry.Entity.InactivatedBy = "";
                        entry.Entity.StatusId = 0;
                        entry.State = EntityState.Modified;
                        break;
                }
            }
            foreach (EntityEntry<ApplicationUser> entry in ChangeTracker.Entries<ApplicationUser>())
            {
                if(entry.State == EntityState.Added)
                {
                    entry.Entity.Id = Guid.NewGuid();
                }
                entry.Entity.FirstName = char.ToUpper(entry.Entity.FirstName[0]) + entry.Entity.FirstName[1..].ToLower();
                entry.Entity.LastName = char.ToUpper(entry.Entity.LastName[0]) + entry.Entity.LastName[1..].ToLower();
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
    }

}

internal sealed class MainDbContextFactory : DesignTimeDbContextFactoryBase<MainDbContext>
{
    protected override MainDbContext CreateNewInstance(DbContextOptions<MainDbContext> options)
    {
        var db = new MainDbContext(options);
        return db;
    }
}
