using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using SaveTheKitty.API.Entities.Users;

namespace SaveTheKitty.API.UnitTests.Common.Factories;

internal static class UserManagerMockFactory
{
    public static UserManager<ApplicationUser> MockUserManager()
    {
        UserManager<ApplicationUser> userManagerMock = Substitute.For<UserManager<ApplicationUser>>(
        Substitute.For<IUserStore<ApplicationUser>>(),
        Substitute.For<IOptions<IdentityOptions>>(),
        Substitute.For<IPasswordHasher<ApplicationUser>>(),
        Array.Empty<IUserValidator<ApplicationUser>>(),
        Array.Empty<IPasswordValidator<ApplicationUser>>(),
        Substitute.For<ILookupNormalizer>(),
        Substitute.For<IdentityErrorDescriber>(),
        Substitute.For<IServiceProvider>(),
        Substitute.For<ILogger<UserManager<ApplicationUser>>>());
        return userManagerMock;
    }
}
