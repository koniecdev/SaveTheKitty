using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using SaveTheKitty.API.Common.Services.Interfaces;
using SaveTheKitty.API.Databases;
using SaveTheKitty.API.Entities.Cats;
using SaveTheKitty.API.Entities.Users;
using SaveTheKitty.API.Exceptions;
using SaveTheKitty.API.Features.Users;
using SaveTheKitty.API.UnitTests.Common;
using SaveTheKitty.API.UnitTests.Common.Factories;
using Shouldly;
using Xunit;

namespace SaveTheKitty.API.UnitTests.Tests.Cats;

public class CreateCatTests : CommandTestBase
{
    private readonly CreateCat.Handler _handler;
    public CreateCatTests() : base()
    {
         _handler = new(_db);
    }

    [Fact]
    public async Task Handle_ShouldCreateCat()
    {
        CreateCat.Command command = new(
            Guid.Parse("2507210c-6946-4652-abf5-b5cb2d2d9b7c"),
            "Krówka",
            Age: 5);

        Guid id = await _handler.Handle(command, CancellationToken.None);

        Cat? result = await _db.Cats.FirstOrDefaultAsync(m => m.Id == id);
        result.ShouldNotBeNull();
        result.Name.ShouldBe(command.Name);
        result.Age.ShouldBe(command.Age);
    }

    [Theory]
    [InlineData("2507210c-6946-4652-2222-b5cb2d2d9b7c")]
    [InlineData("2222210c-6946-4652-abf5-b5cb2d2d9b7c")]
    public void Handle_InputNegative_ThrowsNotFoundException(string guid)
    {
        CreateCat.Command command = new(
            Guid.Parse(guid),
            "Krówka",
            Age: 5);

        Func<Task> result = async () => { await _handler.Handle(command, CancellationToken.None); };

        result.ShouldThrow<NotFoundException>();
    }

}
