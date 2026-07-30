using Asp.Versioning;

namespace OptimalCoder.Blueprint.API.Versioning
{
    public static class OptimalSwaggerExtensions
    {
        public static IServiceCollection AddOptimalSwaggerVersioning(this IServiceCollection services)
        {

            services.AddSwaggerGen();
            services.ConfigureOptions<OptimalSwaggerOptions>();

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();

            })
            .AddApiExplorer(options =>
            {
                options.SubstituteApiVersionInUrl = true;
                options.GroupNameFormat = "'v'VVV";
            });

            

            return services;
        }

        public static void UseOptimalSwagger(this WebApplication app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1.0");
                options.SwaggerEndpoint("/swagger/v1.1/swagger.json", "API v1.1");
                options.SwaggerEndpoint("/swagger/common/swagger.json", "Common API");
            });
        }
    }
}
