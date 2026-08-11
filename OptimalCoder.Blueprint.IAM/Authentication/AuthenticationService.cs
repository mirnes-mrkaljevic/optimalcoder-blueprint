using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OptimalCoder.Blueprint.DB.Context;
using OptimalCoder.Blueprint.DB.Entities;
using OptimalCoder.Blueprint.IAM.Authentication.Model;
using OptimalCoder.Blueprint.Shared.Config;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OptimalCoder.Blueprint.IAM.Authentication
{
    public interface IAuthenticationService
    {
        TokenResponse Login(UserLoginModel user);
        TokenResponse RefreshToken(TokenRequest request);
        bool Logout(string username, TokenRequest request);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserDbContext _userDbContext;
        private readonly IPasswordService _passwordService;
        private readonly Jwt _jwtConfig;

        public AuthenticationService(UserDbContext userDbContext, IPasswordService passwordService, IOptions<AppSettings> appSettings)
        {
            _userDbContext = userDbContext;
            _passwordService = passwordService;
            _jwtConfig = appSettings.Value.Jwt;
        }


        public TokenResponse Login(UserLoginModel model)
        {
            var user = _userDbContext.User.Include(u => u.Roles).FirstOrDefault(x => x.UserName == model.UserName);
            if (user == null)
            {
                throw new UnauthorizedException("INVALID_CREDENTIALS", "Invalid username or password.");
            }

            if (!user.EmailConfirmed)
            {
                throw new UnauthorizedException("EMAIL_NOT_CONFIRMED", "Email not confirmed.");
            }

            var passwordValid = _passwordService.Verify(user, model.Password, user.PasswordHash);

            if (!passwordValid)
            {
                throw new UnauthorizedException("INVALID_CREDENTIALS", "Invalid username or password.");
            }

            var authToken = GenerateAuthToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshTokenHash = HashRefreshToken(refreshToken);
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenValidityInDays);

            _userDbContext.SaveChanges();

            return new TokenResponse
            {
                AuthToken = authToken,
                RefreshToken = refreshToken
            };

        }

        public TokenResponse RefreshToken(TokenRequest request)
        {
            var refreshTokenHash = HashRefreshToken(request.RefreshToken);

            var user = _userDbContext.User.Include(x => x.Roles).FirstOrDefault(x => x.RefreshTokenHash == refreshTokenHash);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new UnauthorizedException("REFRESH_TOKEN_FAILED", "Refresh token failed.");
            }

            var newAuthToken = GenerateAuthToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshTokenHash = HashRefreshToken(newRefreshToken);

            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenValidityInDays);

            _userDbContext.SaveChanges();

            return new TokenResponse
            {
                AuthToken = newAuthToken,
                RefreshToken = newRefreshToken
            };
        }

        public bool Logout(string username, TokenRequest request)
        {
            var refreshTokenHash = HashRefreshToken(request.RefreshToken);

            var user = _userDbContext.User.FirstOrDefault(x => x.UserName == username &&
                x.RefreshTokenHash == refreshTokenHash);

            if (user == null)
            {
                throw new UnauthorizedException("LOGOUT_FAILED", "Invalid refresh token.");
            }

            user.RefreshTokenHash = null;
            user.RefreshTokenExpiryTime = null;

            _userDbContext.SaveChanges();

            return true;
        }

        private string GenerateAuthToken(User user)
        {
            var claims = CreateClaims(user);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            foreach (var audience in _jwtConfig.Audiences)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Aud, audience));
            }

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtConfig.TokenValidityInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        private static string HashRefreshToken(string refreshToken)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));

            return Convert.ToBase64String(hash);
        }

        private List<Claim> CreateClaims(User user)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Name, user.UserName));

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            return claims;
        }
    }
}
