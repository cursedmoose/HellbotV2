using FluentMigrator;

namespace Hellbot.Service.Data.Migrations
{
    [Migration(2026041201)]
    public class M003_CreateUsersTable : Migration
    {
        public override void Up()
        {
            Create.Table("users")
                .WithColumn("id").AsGuid().PrimaryKey()
                .WithColumn("status").AsString()
                .WithColumn("role").AsInt32()
                .WithColumn("joined_at").AsDateTime().Nullable()
                .WithColumn("created_at").AsDateTime();
        }

        public override void Down()
        {
            Delete.Table("users");
        }
    }
}
