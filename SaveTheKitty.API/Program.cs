using Serilog;
using Carter;
using SaveTheKitty.API.Extensions.DependencyInjection;
using SaveTheKitty.API.Exceptions.Handling;
using Microsoft.AspNetCore.Identity;
using SaveTheKitty.API.Entities.Users;
using System.Security.Claims;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddAuthorizationBuilder();

builder.Services.AddCarter();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Origins", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterServices(builder.Configuration);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

WebApplication app = builder.Build();
app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors("MyOrigins");
app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();
app.MapGet("foo", (ClaimsPrincipal principal) => $"xd" ).RequireAuthorization();
app.MapIdentityApi<ApplicationUser>();
app.Run();
