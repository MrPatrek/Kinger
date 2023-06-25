var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// ... we removed Swagger services from here

var app = builder.Build();

// Configure the HTTP request pipeline.

// ... we removed Swagger parts from here

app.MapControllers();

app.Run();
