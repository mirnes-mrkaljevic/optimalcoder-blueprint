using FluentValidation;
using OptimalCoder.Blueprint.API.Validation.Validators.Authentication;
using OptimalCoder.Blueprint.IAM.Authentication.Model;

namespace OptimalCoder.Blueprint.API.Validation
{
    public static class ValidationExtensions
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<UserLoginModel>, UserLoginModelValidator>();
            services.AddScoped<IValidator<TokenRequest>, TokenRequestValidator>();

            return services;
        }
    }
}
