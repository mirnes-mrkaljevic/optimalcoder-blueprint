using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OptimalCoder.Blueprint.Domain.Identity;
using OptimalCoder.Blueprint.Domain.Identity.Models;

namespace OptimalCoder.Blueprint.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "common")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _service;
        public AuthenticationController(IAuthenticationService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] UserModel user)
        {
            var tokenModel = _service.Login(user);
            return Ok(new { TokenModel = tokenModel });
        }

        [HttpPost("RefreshToken")]
        public IActionResult RefreshToken(TokenModel oldTokenModel)
        {

            oldTokenModel.AuthToken = Request.Headers.Authorization.ToString().Replace("Bearer", string.Empty).Trim();
            var newTokenModel = _service.RefreshAuthToken(oldTokenModel);
            return Ok(newTokenModel);

        }
    }
}
