using Microsoft.AspNetCore.Identity;
using OptimalCoder.Blueprint.DB.Entities;

namespace OptimalCoder.Blueprint.IAM.Authentication
{
    public interface IPasswordService
    {
        string Hash(User user, string password);

        bool Verify(User user, string password, string passwordHash);
    }

    public class PasswordService : IPasswordService
    {
        private readonly IPasswordHasher<User> _passwordHasher;

        public PasswordService(IPasswordHasher<User> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public string Hash(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool Verify(User user, string password, string passwordHash)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, passwordHash, password);

            return result != PasswordVerificationResult.Failed;
        }
    }
}
