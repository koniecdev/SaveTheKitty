using Serilog;
using Carter;
using SaveTheKitty.API.Extensions.DependencyInjection;
using SaveTheKitty.API.Exceptions.Handling;

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
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

WebApplication app = builder.Build();
app.UseSerilogRequestLogging();
app.UseExceptionHandler();
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
