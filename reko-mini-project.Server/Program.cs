using reko_mini_project.Server.Configurations;
using reko_mini_project.Server.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AppApplicationServices(builder.Configuration);

var app = builder.Build();

// Preseed the database with sample data in development environment
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await ProductSeeder.SeedAsync(dbContext);
}

app.ConfigureMiddleware();

app.Run();
