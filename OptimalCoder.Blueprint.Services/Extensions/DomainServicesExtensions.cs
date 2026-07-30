using Microsoft.Extensions.DependencyInjection;
using OptimalCoder.Blueprint.Domain.Identity;

namespace OptimalCoder.Blueprint.Domain.Extensions
{
    public static class DomainServicesExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<ICryptoService, CryptoService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();


            return services;
        }
    }
}
