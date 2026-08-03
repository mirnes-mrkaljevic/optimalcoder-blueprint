using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OptimalCoder.Blueprint.DB.Entities;
using OptimalCoder.Blueprint.IAM.Authentication;

namespace OptimalCoder.Blueprint.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "common")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _service;
        private readonly IValidator<UserLoginModel> _loginModelValidator;
        private readonly IValidator<TokenModel> _tokenModelValidator;
        public AuthenticationController(IAuthenticationService service, IValidator<UserLoginModel> loginModelValidator, IValidator<TokenModel> tokenModelValidator)
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

        [Authorize]
        [HttpPost("RefreshToken")]
        public IActionResult RefreshToken([FromBody] TokenModel oldTokenModel)
        {

            var validation = _tokenModelValidator.Validate(oldTokenModel);
            if (!validation.IsValid)
            {
                throw new ValidationException(validation.Errors);
            }

            oldTokenModel.AuthToken = Request.Headers.Authorization.ToString().Replace("Bearer", string.Empty).Trim();
            var newTokenModel = _service.RefreshAuthToken(oldTokenModel);
            return Ok(newTokenModel);
        }

        [Authorize]
        [HttpPost("Logout")]
        public IActionResult Logout([FromBody] TokenModel tokenModel)
        {
            var validation = _tokenModelValidator.Validate(tokenModel);
            if (!validation.IsValid)
            {
                throw new ValidationException(validation.Errors);
            }

            bool success = _service.Logout(tokenModel);
            return Ok(success);
        }
    }
}
