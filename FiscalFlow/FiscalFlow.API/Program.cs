using FiscalFlow.API.Settings;
using FiscalFlow.Infrastructure.Middleware;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddApplication()
    .AddInfrastructure(config);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add exception handling middleware
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseCors(options =>
{
    options.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .WithOrigins(config["JWT:Audience"]!);
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();