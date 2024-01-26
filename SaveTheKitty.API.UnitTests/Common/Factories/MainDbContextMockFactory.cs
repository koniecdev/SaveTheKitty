using Microsoft.EntityFrameworkCore;
using NSubstitute;
using SaveTheKitty.API.Common.Services.Interfaces;
using SaveTheKitty.API.Databases;
using SaveTheKitty.API.Entities.Users;

namespace SaveTheKitty.API.UnitTests.Common.Factories;

public static class MainDbContextMockFactory
{
    public static MainDbContext Create()
    {
        DbContextOptions<MainDbContext> options = new DbContextOptionsBuilder<MainDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        IDateTime dateTime = Substitute.For<IDateTime>();
        dateTime.Now.Returns(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.FromHours(0)));
        MainDbContext dbContext = new(dateTime, options);
        dbContext.ApplicationUsers.Add(new ApplicationUser { Id = Guid.Parse("2507210c-6946-4652-abf5-b5cb2d2d9b7c"), UserName = "default@default.com", Email = "default@default.com", FirstName = "Default", LastName = "Default", PhoneNumber = "111222333" });
        dbContext.SaveChanges();
        return dbContext;
    }
    public static void Destroy(MainDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }
}
