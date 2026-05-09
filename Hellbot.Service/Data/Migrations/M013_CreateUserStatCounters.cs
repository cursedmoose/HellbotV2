using FluentMigrator;

namespace Hellbot.Service.Data.Migrations;

[Migration(2026050902)]
public class M013_CreateUserStatCounters : Migration
{
    public override void Up()
    {
        Create.Table("user_stats")
            .WithColumn("user_id").AsGuid().NotNullable()
            .WithColumn("stat_key").AsString().NotNullable()
            .WithColumn("scope").AsString().NotNullable()
            .WithColumn("value").AsInt64().NotNullable().WithDefaultValue(0)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime);

        Create.PrimaryKey("pk_user_stats")
            .OnTable("user_stats")
            .Columns("user_id", "stat_key", "scope");

        Create.Index("idx_user_stat_scope_stat")
            .OnTable("user_stats")
            .OnColumn("scope").Ascending()
            .OnColumn("stat_key").Ascending();
    }

    public override void Down()
    {
        Delete.Index("idx_user_stat_scope_stat").OnTable("user_stats");

        Delete.Table("user_stats");
    }
}
