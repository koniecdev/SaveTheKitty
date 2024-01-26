using FluentValidation;
using FluentValidation.TestHelper;
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

public class UpdateCatTests : CommandTestBase
{
    private readonly UpdateCat.Handler _handler;
    public UpdateCatTests() : base()
    {
        _handler = new(_db);
    }

    [Fact]
    public async Task Handle_ShouldUpdateCat()
    {
        var fakeCat = new Cat { Id = Guid.NewGuid(), Name = "Fluffy", Age = 2, ApplicationUserId = Guid.Parse("2507210c-6946-4652-abf5-b5cb2d2d9b7c"), HasHealthBook = true, HasTemporaryShelter = true, RequireMedicalHelp = false }; // Sample cat entity
        _db.Cats.Add(fakeCat);
        await _db.SaveChangesAsync(CancellationToken.None);

        var request = new UpdateCat.Command
        {
            Id = fakeCat.Id,
            Name = "Whiskers",
            Age = 3,
            HasTemporaryShelter = true,
            HasHealthBook = true,
            RequireMedicalHelp = false
        };

        await _handler.Handle(request, CancellationToken.None);

        var updatedCat = await _db.Cats.FirstAsync(m=>m.Id == fakeCat.Id);
        updatedCat.ShouldNotBeNull(); // Ensure the cat is found
        updatedCat.Name.ShouldBe("Whiskers"); // Ensure the name is updated
        updatedCat.Age.ShouldBe(3);
    }

    [Fact]
    public async Task Handler_InvalidId_ThrowsCatNotFoundException()
    {
        var invalidId = Guid.NewGuid(); // Invalid ID that doesn't exist in the database
        var request = new UpdateCat.Command
        {
            Id = invalidId,
            Name = "Whiskers",
            Age = 3,
            HasTemporaryShelter = true,
            HasHealthBook = true,
            RequireMedicalHelp = false
        };

        // Act & Assert
        await Assert.ThrowsAsync<CatNotFoundException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public void Validator_ValidRequest_ReturnsTrue()
    {
        // Arrange
        var validator = new UpdateCat.Validator();
        var validRequest = new UpdateCat.Command
        {
            Id = Guid.NewGuid(),
            Name = "Whiskers",
            Age = 3,
            HasTemporaryShelter = true,
            HasHealthBook = true,
            RequireMedicalHelp = false
            // Populate other properties as needed
        };

        // Act
        var result = validator.TestValidate(validRequest);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Id);
        result.ShouldNotHaveValidationErrorFor(r => r.Name);
        result.ShouldNotHaveValidationErrorFor(r => r.Age);
        // Additional assertions for other properties
    }

    [Theory]
    [InlineData("", 3)] // Empty name
    [InlineData("F", 3)] // Name with less than 2 characters
    [InlineData("Very long name that exceeds the maximum length allowed for a cat's name", 3)] // Name exceeds maximum length
    // Add more test cases for other invalid scenarios
    public void Validator_InvalidRequestName_ReturnsError(string name, int age)
    {
        // Arrange
        var validator = new UpdateCat.Validator();
        var invalidRequest = new UpdateCat.Command
        {
            Id = Guid.NewGuid(),
            Name = name,
            Age = age,
            HasTemporaryShelter = true,
            HasHealthBook = true,
            RequireMedicalHelp = false
            // Populate other properties as needed
        };

        // Act
        var result = validator.TestValidate(invalidRequest);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Name);
        result.ShouldNotHaveValidationErrorFor(r => r.Age);
        // Additional assertions for other properties
    }
    [Theory]
    [InlineData("Whiskers", -1)] // Negative age
    [InlineData("Whiskers", 40)] // Age exceeds maximum age allowed for a cat
    public void Validator_InvalidRequestAge_ReturnsError(string name, int age)
    {
        // Arrange
        var validator = new UpdateCat.Validator();
        var invalidRequest = new UpdateCat.Command
        {
            Id = Guid.NewGuid(),
            Name = name,
            Age = age,
            HasTemporaryShelter = true,
            HasHealthBook = true,
            RequireMedicalHelp = false
            // Populate other properties as needed
        };

        // Act
        var result = validator.TestValidate(invalidRequest);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Name);
        result.ShouldHaveValidationErrorFor(r => r.Age);
        // Additional assertions for other properties
    }
}
