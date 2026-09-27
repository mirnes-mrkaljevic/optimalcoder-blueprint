using FluentMigrator;

namespace OptimalCoder.Blueprint.DB.Migrations
{
    [Migration(20260729002)]
    public class Mig20260729002_InitUserData : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            
            Insert.IntoTable("User")
                .Row(new { UserName = "optimalcoderdemo", 
                    PasswordHash = "AQAAAAIAAYagAAAAEOu3iRmufvNc+1tE4N3w/AvdDff+nw8fe/QLM9jPH5qJmL/lJc5mfXn6jJXv3gkPZA==", EmailConfirmed = true, Locked = false });
        }
    }
}
