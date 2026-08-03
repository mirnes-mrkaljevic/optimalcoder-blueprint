using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OptimalCoder.Blueprint.API.Versioning
{
    public class OptimalSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public OptimalSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    new OpenApiInfo
                    {
                        Title = "API " + description.GroupName,
                        Version = description.ApiVersion.ToString()
                    });
            }


            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                return apiDesc.GroupName == docName;

            });

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, CreateAuthScheme());
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document)] = []
            });

        }

        private static OpenApiSecurityScheme CreateAuthScheme()
        {
            return new OpenApiSecurityScheme()
            {
                Name = "JWT Bearer Token",
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT",
                Description = "JWT Bearer Authorization",
            };
        }
    }
}
