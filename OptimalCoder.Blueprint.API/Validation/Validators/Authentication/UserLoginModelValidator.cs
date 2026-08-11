using FluentValidation;
using OptimalCoder.Blueprint.IAM.Authentication.Model;

namespace OptimalCoder.Blueprint.API.Validation.Validators.Authentication
{
    public class UserLoginModelValidator : AbstractValidator<UserLoginModel>
    {
        public UserLoginModelValidator()
        {
            RuleFor(x => x.UserName)
                .NotNull()
                .NotEmpty()
                .MinimumLength(6)
                .Matches("^[a-zA-Z0-9_]+$");

            RuleFor(x => x.Password)
                .NotNull()
                .NotEmpty()
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Must contain uppercase")
                .Matches("[a-z]").WithMessage("Must contain lowercase")
                .Matches("[0-9]").WithMessage("Must contain a digit")
                .Matches("[^a-zA-Z0-9]").WithMessage("Must contain a special char");
 
        }
    }
}
