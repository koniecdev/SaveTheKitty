using FluentValidation;
using Microsoft.AspNetCore.Identity;
using SaveTheKitty.API.Common.Behaviours;
using SaveTheKitty.API.Common.Services;
using SaveTheKitty.API.Common.Services.Interfaces;
using SaveTheKitty.API.Databases;
using SaveTheKitty.API.Entities.Users;
using System.Reflection;

namespace SaveTheKitty.API.Extensions.DependencyInjection;

internal static class DependencyInjection
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
            .AddOpenRequestPreProcessor(typeof(LoggingBehaviour<>))
            .AddOpenBehavior(typeof(ValidationBehaviour<,>)));

        services.AddScoped<IDateTime, DateTimeProvider>();
        services.AddDbContext<MainDbContext>(o => o.UseSqlServer(configuration.GetConnectionString("Database") ?? throw new Exception("Connection string not found")));
        services.AddScoped<IMainDbContext, MainDbContext>();
        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<MainDbContext>()
            .AddApiEndpoints();

        return services;
    }
}
