using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OptimalCoder.Blueprint.DB.Entities;
using OptimalCoder.Blueprint.IAM.Authentication;
using OptimalCoder.Blueprint.IAM.Authentication.Model;

namespace OptimalCoder.Blueprint.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "common")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _service;
        private readonly IValidator<UserLoginModel> _loginModelValidator;
        private readonly IValidator<TokenRequest> _tokenModelValidator;
        public AuthenticationController(IAuthenticationService service, IValidator<UserLoginModel> loginModelValidator, IValidator<TokenRequest> tokenModelValidator)
        {
            _service = service;
            _loginModelValidator = loginModelValidator;
            _tokenModelValidator = tokenModelValidator;

        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] UserLoginModel user)
        {
            var validation = _loginModelValidator.Validate(user);
            if (!validation.IsValid)
            {
                throw new ValidationException(validation.Errors);
            }

            var tokenModel = _service.Login(user);
            return Ok(new { TokenModel = tokenModel });
        }

        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public IActionResult RefreshToken([FromBody] TokenRequest request)
        {
            var validation = _tokenModelValidator.Validate(request);

            if (!validation.IsValid)
            {
                throw new ValidationException(validation.Errors);
            }

            var tokenResponse = _service.RefreshToken(request);

            return Ok(tokenResponse);
        }

        [Authorize]
        [HttpPost("Logout")]
        public IActionResult Logout([FromBody] TokenRequest tokenModel)
        {
            var validation = _tokenModelValidator.Validate(tokenModel);
            if (!validation.IsValid)
            {
                throw new ValidationException(validation.Errors);
            }

            var success = _service.Logout(User.Identity?.Name!, tokenModel);

            return Ok(success);
        }
    }
}
