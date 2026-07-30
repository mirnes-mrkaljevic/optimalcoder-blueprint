using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace OptimalCoder.Blueprint.DB.Migrations
{
    public static class MigrationExtensions
    {
        public static IServiceCollection ConfigureOptimalMigrations(this IServiceCollection services, string connectionString)
        {
            // Configure FluentMigrator using the separate migrations project
            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(Mig2026063001_Init).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole());

            return services;
        }

        public static void RunPendingOptimalMigrations(this IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.MigrateUp();
            }
        }
    }
}
