using Microsoft.EntityFrameworkCore;
using OptimalCoder.Blueprint.DB.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OptimalCoder.Blueprint.DB.Context
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
       : base(options)
        {
        }

        public bool UpdateTokens(int userId, string authToken, string refreshToken, DateTime refreshTokenExpiryTime)
        {
            User.Update(new User()
            {
                Id = userId,
                AuthToken = authToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = refreshTokenExpiryTime
            });

            return SaveChanges() == 1;
                
 
        }

        public DbSet<User> User { get; set; }
    }
}
