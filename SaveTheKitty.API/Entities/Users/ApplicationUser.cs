using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SaveTheKitty.API.Entities.Users;
public class ApplicationUser : IdentityUser<string>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string Name => $"{FirstName} {LastName}";
}

internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(m => m.FirstName).IsRequired();
        builder.Property(m => m.LastName).IsRequired();
        builder.Property(m => m.Email).IsRequired();
        builder.Property(m => m.UserName).IsRequired();
    }
}