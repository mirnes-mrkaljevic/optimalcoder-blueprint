using FluentMigrator;
using FluentMigrator.SqlServer;

namespace OptimalCoder.Blueprint.DB.Migrations
{
    [Migration(20260729001)]
    public class Mig20260729001_InitUsers : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Create.Table("User")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("UserName").AsString().Unique()
                .WithColumn("PasswordHash").AsString()
                .WithColumn("PasswordSalt").AsString()
                .WithColumn("AuthToken").AsString(1024).Nullable()
                .WithColumn("RefreshToken").AsString(1024).Nullable()
                .WithColumn("RefreshTokenExpiryTime").AsDateTime().Nullable()
                .WithColumn("SignInBy").AsString().Nullable()
                .WithColumn("EmailConfirmed").AsBoolean()
                .WithColumn("Locked").AsBoolean()
                .WithColumn("CompanyId").AsInt32().Nullable();

            Create.Table("Role")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString().Unique();

            Create.Table("UserRole")
               .WithColumn("UserId").AsInt32().ForeignKey("User", "Id")
               .WithColumn("RoleId").AsInt32().ForeignKey("Role", "Id");
        }
    }
}
