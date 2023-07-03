using API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container;               mine: here the order does not matter.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline;         mine: here the order MATTERS

app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod()
    .WithOrigins("https://localhost:4200"));

app.UseAuthentication();        // Do you have a valid token?           E.g., you go to a night club.   Do you have a valid ID (not a fake one, but true one)?
app.UseAuthorization();         // What are you allowed to do?                                          Are you over 18?

app.MapControllers();

app.Run();
