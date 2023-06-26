using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt => 
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// ... we removed Swagger services from here

var app = builder.Build();

// Configure the HTTP request pipeline.

// ... we removed Swagger parts from here

app.MapControllers();

app.Run();
