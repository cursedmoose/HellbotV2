using FluentMigrator;

namespace Hellbot.Service.Data.Migrations;

[Migration(2026050902)]
public class M013_CreateUserStatCounters : Migration
{
    /// <summary>
    /// SQLite rejects <c>ALTER TABLE … ADD CONSTRAINT … PRIMARY KEY</c> that FluentMigrator emits for a
    /// separate primary-key constraint. Declare PK inline in CREATE TABLE instead.
    /// </summary>
    public override void Up()
    {
        Execute.Sql("""
            CREATE TABLE user_stats (
              user_id TEXT NOT NULL,
              stat_key TEXT NOT NULL,
              scope TEXT NOT NULL,
              value INTEGER NOT NULL DEFAULT 0,
              updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
              PRIMARY KEY (user_id, stat_key, scope)
            );
            """);

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
