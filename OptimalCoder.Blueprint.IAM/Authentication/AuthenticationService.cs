using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OptimalCoder.Blueprint.DB.Context;
using OptimalCoder.Blueprint.DB.Entities;
using OptimalCoder.Blueprint.Shared.Config;
using OptimalCoder.Blueprint.Shared.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OptimalCoder.Blueprint.IAM.Authentication
{
    public interface IAuthenticationService
    {
        TokenModel Login(UserLoginModel user);
        bool Logout(TokenModel tokenModel);
        TokenModel RefreshAuthToken(TokenModel oldTokenModel);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserDbContext _userDbContext;
        private readonly Jwt _jwtConfig;

        public AuthenticationService(UserDbContext userDbContext, IOptions<AppSettings> appSettings)
        {
            _userDbContext = userDbContext;
            _jwtConfig = appSettings.Value.Jwt;
        }


        public TokenModel Login(UserLoginModel usr)
        {
            var user = _userDbContext.User.Include(u => u.Roles).First(x => x.UserName == usr.UserName);
            if (user.EmailConfirmed)
            {
                var salt = Convert.FromBase64String(user.PasswordSalt);
                string hashed = GetPasswordHash(usr.Password, salt);
                if (hashed == user.PasswordHash)
                {
                    var authToken = GenerateAuthToken(user);
                    var refreshToken = GenerateRefreshToken();

                    UpdateTokens(user, authToken, refreshToken);

                    return new TokenModel
                    {
                        AuthToken = authToken,
                        RefreshToken = refreshToken
                    };
                }
                else
                {
                    throw new UnauthorizedException("WRONG_PASSWORD","Wrong password");
                }

            }
            else
            {
                throw new UnauthorizedException("EMAIL_NOT_CONFIRMED", "Email not confirmed!");
            }

        }

        private void UpdateTokens(User user, string authToken, string? refreshToken = null)
        {

            user.AuthToken = authToken;
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                user.RefreshToken = refreshToken;

                var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenValidityInDays);
                user.RefreshTokenExpiryTime = refreshTokenExpiryTime;
            }

            _userDbContext.SaveChanges();
        }
        public TokenModel RefreshAuthToken(TokenModel oldTokenModel)
        {
            var user = _userDbContext.User.Include(u => u.Roles).First(x => x.AuthToken == oldTokenModel.AuthToken && x.RefreshToken == oldTokenModel.RefreshToken);

            if (user?.RefreshTokenExpiryTime > DateTime.UtcNow)
            {
                var newAuthToken = GenerateAuthToken(user);
                UpdateTokens(user, newAuthToken);

                return new TokenModel
                {
                    AuthToken = newAuthToken
                };
            }

            //threat logout when token refresh fails
            throw new UnauthorizedException("REFRESH_TOKEN_FAILED", "Refresh token failed!");

        }

        private string GetPasswordHash(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
               password: password!,
               salt: salt,
               prf: KeyDerivationPrf.HMACSHA256,
               iterationCount: 100000,
               numBytesRequested: 256 / 8));
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

       

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public bool Logout(TokenModel tokenModel)
        {
            var user = _userDbContext.User.First(x => x.AuthToken == tokenModel.AuthToken && x.RefreshToken == tokenModel.RefreshToken);
            user.RefreshTokenExpiryTime = DateTime.UtcNow;
            _userDbContext.SaveChanges();
            return true;
        }
    }
}
