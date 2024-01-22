using Microsoft.EntityFrameworkCore;
using NSubstitute;
using SaveTheKitty.API.Common.Services.Interfaces;
using SaveTheKitty.API.Databases;
using SaveTheKitty.API.Entities.Users;
using SaveTheKitty.API.Features.Users;
using SaveTheKitty.API.UnitTests.Common.Factories;
using Shouldly;
using Xunit;

namespace SaveTheKitty.API.UnitTests.Tests.Cats;

public class CreateCatTests
{
    [Fact]
    public async Task Handle_ShouldCreateCat()
    {
        using MainDbContext context = MainDbContextMockFactory.Create();
        CreateCat.Command command = new(
            Guid.Parse("2507210c-6946-4652-abf5-b5cb2d2d9b7c"),
            "Krówka",
            false,
            false,
            false,
            5
            );
        CreateCat.Handler handler = new(context);

        Guid result = await handler.Handle(command, CancellationToken.None);

        await context.Cats.FirstOrDefaultAsync(m => m.Id == result).ShouldNotBeNull();
    }
}
