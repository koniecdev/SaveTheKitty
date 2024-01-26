using Riok.Mapperly.Abstractions;
using SaveTheKitty.API.Entities.Users;

namespace SaveTheKitty.API.Contracts.Responses;

public sealed class UserResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Username { get; init; }
    public string? Phone { get; init; } = null;
}

[Mapper]
public static partial class GetUsersMapper
{
    public static partial IQueryable<UserResponse> ProjectToDto(this IQueryable<ApplicationUser> q);

    [MapProperty(nameof(ApplicationUser.UserName), nameof(UserResponse.Username))]
    [MapProperty(nameof(ApplicationUser.PhoneNumber), nameof(UserResponse.Phone))]
    private static partial UserResponse Map(ApplicationUser x);
}