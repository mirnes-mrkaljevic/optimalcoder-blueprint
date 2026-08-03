using Microsoft.Extensions.Options;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using OptimalCoder.Blueprint.DB.Context;
using OptimalCoder.Blueprint.DB.Entities;
using OptimalCoder.Blueprint.IAM.Authentication;
using OptimalCoder.Blueprint.Shared.Config;
using System.IdentityModel.Tokens.Jwt;

namespace OptimalCoder.Blueprint.Tests.UnitTests.IAM
{
    [TestFixture]
    public class AuthenticationTests
    {
        private AuthenticationService _authService;
        private User _user;
        private Mock<UserDbContext> _userContextMock;

        private readonly IOptions<AppSettings> _appSettings = Options.Create(new AppSettings()
        {
            Jwt = new Jwt()
            {
                Audiences = ["audience"],
                Issuer = "issuer",
                Key = "my-super-secret-key-with-more-than-16-characters",
                RefreshTokenValidityInDays = 10,
                TokenValidityInMinutes = 2
            }
        });

        [SetUp]
        public void Init()
        {
            _user = new()
            {
                Id = 1,
                EmailConfirmed = true,
                Locked = false,
                AuthToken = "testauthtoken",
                PasswordHash = "PasswordHash",
                PasswordSalt = "PasswordSalt",
                RefreshToken = "RefreshToken",
                UserName = "UserName",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(1),
                Roles = new List<Role>() { new Role()
            {
                Id = 1,
                Name= "User"
            } }
            };
            _userContextMock = new Mock<UserDbContext>();
            _userContextMock.SetupGet(x => x.User).ReturnsDbSet([ _user ]);
            _authService = new AuthenticationService(_userContextMock.Object, _appSettings);
        }

        [Test]
        public void Login_WhenEmailNotConfirmed_ThrowEx()
        {
            _user.EmailConfirmed = false;

            var ex = Assert.Throws<UnauthorizedException>(() => _authService.Login(new UserLoginModel()
            {
                UserName = "UserName",
                Password = "password"
            }))!;

            Assert.AreEqual("EMAIL_NOT_CONFIRMED", ex.Code);
        }

        [Test]
        public void Login_WhenDifferentPasswordHash_ThrowEx()
        {
            var ex = Assert.Throws<UnauthorizedException>(() => _authService.Login(new UserLoginModel()
            {
                UserName = "UserName",
                Password = "password"
            }))!;
            Assert.AreEqual("WRONG_PASSWORD", ex.Code);
        }

        [Test]
        public void RefreshAuthToken_WhenExpired_ThrowEx()
        {
            _user.RefreshTokenExpiryTime = DateTime.UtcNow.AddSeconds(-1);
            var ex = Assert.Throws<UnauthorizedException>(() => _authService.RefreshAuthToken(new TokenModel()
            {
                AuthToken = _user.AuthToken,
                RefreshToken = _user.RefreshToken
            }))!;
            Assert.AreEqual("REFRESH_TOKEN_FAILED", ex.Code);
        }

        [Test]
        public void RefreshAuthToken_WhenSucess_ToReturnNewToken()
        {
           var newToken =  _authService.RefreshAuthToken(new TokenModel()
            {
                AuthToken = _user.AuthToken,
                RefreshToken = _user.RefreshToken
            });

            bool isJwt = new JwtSecurityTokenHandler().CanReadToken(newToken.AuthToken);

            Assert.IsTrue(isJwt);
        }

        [Test]
        public void Logout_WhenSucess_ToReturnTrue()
        {
            var result = _authService.Logout(new TokenModel()
            {
                AuthToken = _user.AuthToken,
                RefreshToken = _user.RefreshToken
            });

            Assert.IsTrue(result);
        }
    }
}
