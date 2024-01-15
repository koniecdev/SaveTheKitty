using Serilog;
using Carter;
using SaveTheKitty.API.Extensions.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

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

WebApplication app = builder.Build();
app.UseSerilogRequestLogging();
app.UseCors("MyOrigins");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/health", (ILogger<Program> logger) =>
{
    logger.LogInformation("workin");
});
//app.UseAuthentication();
//app.UseAuthorization();
app.MapCarter();
app.Run();
