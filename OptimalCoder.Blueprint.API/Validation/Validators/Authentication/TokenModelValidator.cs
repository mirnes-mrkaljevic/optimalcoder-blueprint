using FluentValidation;
using OptimalCoder.Blueprint.IAM.Authentication;

namespace OptimalCoder.Blueprint.API.Validation.Validators.Authentication
{
    public class TokenModelValidator : AbstractValidator<TokenModel>
    {
        public TokenModelValidator()
        {
            RuleFor(x => x.AuthToken).NotNull().NotEmpty().MinimumLength(10);
            RuleFor(x => x.RefreshToken).NotNull().NotEmpty().MinimumLength(8);

        }
    }
}
