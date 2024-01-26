using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaveTheKitty.API.Entities.Cats;

namespace SaveTheKitty.API.Entities.Users;
public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Name => $"{FirstName} {LastName}";
    public ICollection<Cat> Cats { get; } = new List<Cat>();
}

internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(m => m.FirstName).IsRequired();
        builder.Property(m => m.LastName).IsRequired();
        builder.Property(m => m.Email).IsRequired();
        builder.Property(m => m.UserName).IsRequired();
        builder.Property(m => m.Id).ValueGeneratedOnAdd();
        builder.HasIndex(m => m.Email).IsUnique();
    }
}