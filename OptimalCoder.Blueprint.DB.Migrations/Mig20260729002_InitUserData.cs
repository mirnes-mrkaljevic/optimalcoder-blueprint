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
                .Row(new { UserName = "optimalcoderadmin", PasswordHash = "gmDSu/JzrE9nMA40AtMfAwAJ59CcWV6i7fX0TEy7fj0=", PasswordSalt = "I7k67xQ/y2HqsEMIuB95zA==", EmailConfirmed = true, Locked = false });
            Insert.IntoTable("Role").Row(new { Name = "Admin" });


            Execute.Sql(@"
                        INSERT INTO [UserRole] (UserId, RoleId)
                        SELECT u.Id, r.Id
                        FROM [User] u
                        CROSS JOIN [Role] r
                        WHERE u.UserName = 'admin'
                          AND r.Name = 'Admin';
                    ");
        }
    }
}
