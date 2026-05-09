using FluentMigrator;

namespace Hellbot.Service.Data.Migrations;

[Migration(2026050901)]
public class M012_IndexUserIdentityPlatformUsername : Migration
{
    public override void Up()
    {
        Create.Index("idx_user_identities_platform_username")
            .OnTable("user_identities")
            .OnColumn("platform").Ascending()
            .OnColumn("platform_user_name").Ascending();
    }

    public override void Down()
    {
        Delete.Index("idx_user_identities_platform_username").OnTable("user_identities");
    }
}
