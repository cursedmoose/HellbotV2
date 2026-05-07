using FluentMigrator;

namespace Hellbot.Service.Data.Migrations
{
    [Migration(2026050601)]
    public class M007_AddStreamIdToEvents : Migration
    {
        public override void Up()
        {
            Alter.Table("events")
                .AddColumn("stream_id").AsString().Nullable();

            Create.Index("ix_events_stream_id")
                .OnTable("events")
                .OnColumn("stream_id");
        }

        public override void Down()
        {
            Delete.Index("ix_events_stream_id").OnTable("events");

            Delete.Column("stream_id").FromTable("events");
        }
    }
}
