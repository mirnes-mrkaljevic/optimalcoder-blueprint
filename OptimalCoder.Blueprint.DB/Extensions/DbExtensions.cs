using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OptimalCoder.Blueprint.DB.Context;

namespace OptimalCoder.Blueprint.DB.Extensions
{
    public static class DbExtensions
    {
        public static IServiceCollection AddOptimalDbContext(this IServiceCollection services, string connectionString)
        {

            services.AddDbContext<UserDbContext>(options => options.UseSqlServer(connectionString));

            return services;
        }
    }
}
