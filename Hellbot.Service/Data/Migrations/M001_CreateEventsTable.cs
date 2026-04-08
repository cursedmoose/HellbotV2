using FluentMigrator;

namespace Hellbot.Service.Data.Migrations
{
    [Migration(2026040701)]
    public class M001_CreateEventsTable : Migration
    {
        public override void Up()
        {
            Create.Table("events")
                .WithColumn("id").AsString().PrimaryKey()
                .WithColumn("timestamp").AsDateTime().NotNullable()
                .WithColumn("platform").AsString().NotNullable()
                .WithColumn("event_type").AsString().NotNullable()
                .WithColumn("metadata").AsCustom("JSON");
        }

        public override void Down()
        {
            Delete.Table("events");
        }
    }
}
