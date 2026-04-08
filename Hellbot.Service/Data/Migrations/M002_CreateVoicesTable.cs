using FluentMigrator;

namespace Hellbot.Service.Data.Migrations
{
    [Migration(2026040702)]
    public class M002_CreateVoicesTable: Migration
    {
        public override void Up()
        {
            Create.Table("voices")
                .WithColumn("id").AsString().PrimaryKey()
                .WithColumn("name").AsString().NotNullable()
                .WithColumn("settings").AsCustom("JSON");
        }

        public override void Down()
        {
            Delete.Table("voices");
        }
    }
}
