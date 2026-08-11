using FluentValidation;
using OptimalCoder.Blueprint.IAM.Authentication.Model;

namespace OptimalCoder.Blueprint.API.Validation.Validators.Authentication
{
    public class TokenRequestValidator : AbstractValidator<TokenRequest>
    {
        public TokenRequestValidator()
        {
            RuleFor(x => x.RefreshToken).NotNull().NotEmpty().MinimumLength(8);

        }
    }
}
