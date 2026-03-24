using reko_mini_project.Server.Configurations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AppApplicationServices(builder.Configuration);

var app = builder.Build();
app.ConfigureMiddleware();

app.Run();
