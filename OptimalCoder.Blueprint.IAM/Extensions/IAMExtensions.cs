using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OptimalCoder.Blueprint.DB.Entities;
using OptimalCoder.Blueprint.IAM.Authentication;
using OptimalCoder.Blueprint.Shared.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace OptimalCoder.Blueprint.IAM.Extensions
{
    public static class IAMExtensions
    {
        public static IServiceCollection AddIAMServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IPasswordService, PasswordService>();

            return services;
        }

        public static IServiceCollection AddOptimalAuthentication(this IServiceCollection services, Jwt jwt)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(jwtOptions =>
                    {
                        jwtOptions.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidAudiences = jwt.Audiences,
                            ValidIssuer = jwt.Issuer,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
                        };
                    });

            return services;
        }
    }
}
