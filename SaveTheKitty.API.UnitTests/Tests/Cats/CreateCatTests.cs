using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SaveTheKitty.API.Entities.Cats;
using SaveTheKitty.API.Exceptions;
using SaveTheKitty.API.Features.Users;
using SaveTheKitty.API.UnitTests.Common;
using Shouldly;
using System;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace SaveTheKitty.API.UnitTests.Tests.Cats;

public class CreateCatTests : CommandTestBase
{
    private readonly CreateCat.Handler _handler;
    private readonly CreateCat.Command _command;
    public CreateCatTests() : base()
    {
        _handler = new(_db);
        _command = new(
            ApplicationUserId: Guid.Parse("2507210c-6946-4652-abf5-b5cb2d2d9b7c"),
            Name: "Krówka",
            HasTemporaryShelter: true,
            Age: 5);
    }

    [Fact]
    public async Task Handle_ShouldCreateCat()
    {
        CreateCat.Command command = _command;

        Guid id = await _handler.Handle(command, CancellationToken.None);

        Cat? result = await _db.Cats.FirstOrDefaultAsync(m => m.Id == id);
        result.ShouldNotBeNull();
        result.Name.ShouldBe(command.Name);
        result.Age.ShouldBe(command.Age);
    }

    [Theory]
    [InlineData("2507210c-6946-4652-2222-b5cb2d2d9b7c")]
    [InlineData("2222210c-6946-4652-abf5-b5cb2d2d9b7c")]
    public void Handle_InputNegative_ThrowsNotFoundException(Guid guid)
    {
        CreateCat.Command command = _command with { ApplicationUserId = guid };
        Func<Task> result = async () => { await _handler.Handle(command, CancellationToken.None); };

        result.ShouldThrow<NotFoundException>();
    }

    [Fact]
    public void Validate_InputNegative_ThrowsException()
    {
        CreateCat.Command command = new(Guid.Parse("2222210c-6946-4652-abf5-b5cb2d2d9b7c"), "", true, Age: 2222);
        CreateCat.Validator validator = new();

        var results = validator.Validate(command);

        results.Errors.Count.ShouldBe(3);
    }

}
