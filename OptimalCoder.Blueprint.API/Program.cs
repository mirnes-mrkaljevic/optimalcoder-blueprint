using Asp.Versioning.Conventions;
using OptimalCoder.Blueprint.API.Exceptions;
using OptimalCoder.Blueprint.API.Versioning;
using OptimalCoder.Blueprint.DB.Extensions;
using OptimalCoder.Blueprint.DB.Migrations;
using OptimalCoder.Blueprint.Domain.Extensions;
using OptimalCoder.Blueprint.Infra;
using OptimalCoder.Blueprint.Infra.Logger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString(nameof(ConnectionStrings.DefaultConnection))!;
builder.Services.ConfigureOptimalMigrations(connectionString);

builder.Services.Configure<AppSettings>(builder.Configuration);

builder.Logging.ClearProviders().AddOptimalLogger();

builder.Services.AddOptimalSwaggerVersioning();
builder.Services.AddOptimalDbContext(connectionString);
builder.Services.AddDomainServices();


var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Apply pending migrations on startup
app.Services.RunPendingOptimalMigrations();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOptimalSwagger();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
