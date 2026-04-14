using FluentMigrator;

namespace Hellbot.Service.Data.Migrations
{
    [Migration(2026041202)]
    public class M004_CreateUserIdentities : Migration
    {
        public override void Up()
        {
            Create.Table("user_identities")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("user_id").AsGuid()
                .WithColumn("platform").AsString()
                .WithColumn("platform_user_id").AsString()
                .WithColumn("platform_user_name").AsString()
                .WithColumn("linked_at").AsDateTime();

            Create.UniqueConstraint("uq_platform_identity")
                .OnTable("user_identities")
                .Columns("platform", "platform_user_id");

            Create.Index("idx_user_identities_user")
                .OnTable("user_identities")
                .OnColumn("user_id");
        }

        public override void Down()
        {
            Delete.Table("user_identities");

        }
    }
}
