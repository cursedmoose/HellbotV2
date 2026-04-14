using FluentMigrator;

namespace Hellbot.Service.Data.Migrations
{
    [Migration(2026041301)]
    public class M005_CreateUserEntitlements : Migration
    {
        public override void Up()
        {
            Create.Table("user_entitlements")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("user_id").AsGuid()
                .WithColumn("type").AsString()
                .WithColumn("value").AsString()
                .WithColumn("earned_at").AsDateTime();

            Create.Index("idx_user_entitlements_user")
                .OnTable("user_entitlements")
                .OnColumn("user_id");

            Create.Index("idx_user_entitlements_type")
                .OnTable("user_entitlements")
                .OnColumn("type");
        }

        public override void Down()
        {
            Delete.Table("user_entitlements");
        }
    }
}
