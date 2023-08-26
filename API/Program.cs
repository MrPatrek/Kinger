using API.Data;
using API.Data.Migrations;
using API.Entities;
using API.Extensions;
using API.Middleware;
using API.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container;               mine: here the order does not matter.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline;         mine: here the order MATTERS
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(builder => builder
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()         // for SignalR
                            .WithOrigins("https://localhost:4200"
));

app.UseAuthentication();        // Do you have a valid token?           E.g., you go to a night club.   Do you have a valid ID (not a fake one, but true one)?
app.UseAuthorization();         // What are you allowed to do?                                          Are you over 18?

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");

// Seed the user data to the database:
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(userManager, roleManager);
}
catch (Exception ex)
{
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");
}

app.Run();
