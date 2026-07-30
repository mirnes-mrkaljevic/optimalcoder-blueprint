using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OptimalCoder.Blueprint.DB.Context;
using OptimalCoder.Blueprint.DB.Entities;
using OptimalCoder.Blueprint.Domain.Identity.Models;
using OptimalCoder.Blueprint.Shared.Config;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OptimalCoder.Blueprint.Domain.Identity
{
    public interface IAuthenticationService
    {
        TokenModel Login(UserModel user);
        string GenerateAuthToken(int userId);
        string GenerateRefreshToken();
        bool ValidateToken(string token);
        TokenModel RefreshAuthToken(TokenModel oldTokenModel);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserDbContext _userDbContext;
        private readonly ICryptoService _cryptoService;
        private readonly Jwt _jwtConfig;

        public AuthenticationService(UserDbContext userDbContext, ICryptoService cryptoService, IOptions<Jwt> jwt)
        {
            _userDbContext = userDbContext;
            _cryptoService = cryptoService;
            _jwtConfig = jwt.Value;
        }


        public TokenModel Login(UserModel usr)
        {
            var user = _userDbContext.User.First(x => x.UserName == usr.Email);
            if (user.EmailConfirmed)
            {
                var salt = Convert.FromBase64String(user.PasswordSalt);
                string hashed = GetPasswordHash(usr.Password, salt);
                if (hashed == user.PasswordHash)
                {
                    var authToken = GenerateAuthToken(user.Id);
                    var refreshToken = GenerateRefreshToken();
                    var refreshTokenExpiredTime = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenValidityInDays);
                    _userDbContext.UpdateTokens(user.Id, authToken, refreshToken, refreshTokenExpiredTime);

                    return new TokenModel
                    {
                        AuthToken = authToken,
                        RefreshToken = refreshToken
                    };
                }
                else
                {
                    throw new UnauthorizedAccessException("Wrong password");
                }

            }
            else
            {
                throw new UnauthorizedAccessException("Email not confirmed!");
            }



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

        
        public string GenerateAuthToken(int userId)
        {
            var claims = CreateClaims(userId);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer: _jwtConfig.Issuer, audience: _jwtConfig.Audience, claims,
                expires: DateTime.Now.AddMinutes(_jwtConfig.TokenValidityInMinutes), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        private List<Claim> CreateClaims(int userId)
        {
            var claims = new List<Claim>();

            var user = _userDbContext.User.First(x => x.Id == userId);
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            return claims;
        }

        public bool ValidateToken(string token)
        {
            var secret = Encoding.UTF8.GetBytes(_jwtConfig.Key);
            var mySecurityKey = new SymmetricSecurityKey(secret);
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _jwtConfig.Issuer,
                    ValidAudience = _jwtConfig.Audience,
                    IssuerSigningKey = mySecurityKey,
                    RequireExpirationTime= true,
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public TokenModel RefreshAuthToken(TokenModel oldTokenModel)
        {
            var user = _userDbContext.User.First(x => x.AuthToken == oldTokenModel.AuthToken && x.RefreshToken == oldTokenModel.RefreshToken);

            if (user?.RefreshTokenExpiryTime > DateTime.UtcNow)
            {            
                var newAuthToken = GenerateAuthToken(user.Id);
                _userDbContext.User.Update(new User() { Id = user.Id, AuthToken = newAuthToken });

                return new TokenModel
                {
                    AuthToken = newAuthToken
                };
            }
  
            //threat logout when token refresh fails
            throw new UnauthorizedAccessException("Refresh token failed!");

        }

       
    }
}
