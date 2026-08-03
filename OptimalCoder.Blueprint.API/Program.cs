using OptimalCoder.Blueprint.API.Exceptions;
using OptimalCoder.Blueprint.API.RateLimiting;
using OptimalCoder.Blueprint.API.Validation;
using OptimalCoder.Blueprint.API.Versioning;
using OptimalCoder.Blueprint.DB.Extensions;
using OptimalCoder.Blueprint.DB.Migrations;
using OptimalCoder.Blueprint.Domain.Extensions;
using OptimalCoder.Blueprint.IAM.Extensions;
using OptimalCoder.Blueprint.Infra.Extensions;
using OptimalCoder.Blueprint.Infra.Logger;
using OptimalCoder.Blueprint.Shared.Config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.Configure<AppSettings>(builder.Configuration);
var connectionString = builder.Configuration.GetConnectionString(nameof(ConnectionStrings.DefaultConnection))!;

builder.Services.ConfigureOptimalMigrations(connectionString);
builder.Logging.ClearProviders().AddOptimalLogger();
builder.Services.AddOptimalSwaggerVersioning();
builder.Services.AddOptimalDbContext(connectionString);
builder.Services.AddDomainServices();
builder.Services.AddInfraServices();
builder.Services.AddIAMServices();
builder.Services.AddOptimalRateLimiter();
builder.Services.AddValidators();

builder.Services.AddOptimalAuthentication(builder.Configuration.Get<AppSettings>()!.Jwt);
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseRateLimiter();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Services.RunPendingOptimalMigrations();

if (app.Environment.IsDevelopment())
{
    app.UseOptimalSwagger();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
