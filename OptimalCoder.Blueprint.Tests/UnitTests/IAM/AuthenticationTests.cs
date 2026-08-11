using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Options;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using OptimalCoder.Blueprint.DB.Context;
using OptimalCoder.Blueprint.DB.Entities;
using OptimalCoder.Blueprint.IAM.Authentication;
using OptimalCoder.Blueprint.IAM.Authentication.Model;
using OptimalCoder.Blueprint.Shared.Config;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace OptimalCoder.Blueprint.Tests.UnitTests.IAM
{
    [TestFixture]
    public class AuthenticationTests
    {
        private AuthenticationService _authService;
        private User _user;
        private Mock<UserDbContext> _userContextMock;
        private Mock<IPasswordService> _passwordServiceMock;

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
                RefreshTokenHash = "RefreshTokenHash",
                PasswordHash = "PasswordHash",
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

            _passwordServiceMock = new Mock<IPasswordService>();
            _authService = new AuthenticationService(_userContextMock.Object, _passwordServiceMock.Object, _appSettings);
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
            _passwordServiceMock.Setup(x => x.Hash(_user, "password")).Returns("wrong password hash");

            var ex = Assert.Throws<UnauthorizedException>(() => _authService.Login(new UserLoginModel()
            {
                UserName = _user.UserName,
                Password = "password"
            }))!;
            Assert.AreEqual("INVALID_CREDENTIALS", ex.Code);
        }

        [Test]
        public void RefreshAuthToken_WhenExpired_ThrowEx()
        {
            _user.RefreshTokenExpiryTime = DateTime.UtcNow.AddSeconds(-1);
            var ex = Assert.Throws<UnauthorizedException>(() => _authService.RefreshToken(new TokenRequest()
            {
                RefreshToken = "refresh token"
            }))!;
            Assert.AreEqual("REFRESH_TOKEN_FAILED", ex.Code);
        }

        [Test]
        public void RefreshAuthToken_WhenSucess_ToReturnNewToken()
        {
            var refreshToken = "refreshToken";
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
            _user.RefreshTokenHash = Convert.ToBase64String(hash);

            var newToken =  _authService.RefreshToken(new TokenRequest()
            {
                RefreshToken = refreshToken
            });

            bool isJwt = new JwtSecurityTokenHandler().CanReadToken(newToken.AuthToken);

            Assert.IsTrue(isJwt);
        }

        [Test]
        public void Logout_WhenSucess_ToReturnTrue()
        {
            var refreshToken = "refreshToken";
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
            _user.RefreshTokenHash = Convert.ToBase64String(hash);

            var result = _authService.Logout(_user.UserName, new TokenRequest()
            {
                RefreshToken = refreshToken
            });

            Assert.IsTrue(result);
        }
    }
}
