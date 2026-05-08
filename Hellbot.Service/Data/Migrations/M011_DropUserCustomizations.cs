using FluentMigrator;

namespace Hellbot.Service.Data.Migrations;

[Migration(2026050803)]
public class M011_DropUserCustomizations : Migration
{
    public override void Up()
    {
        Delete.Table("user_customizations");
    }

    public override void Down()
    {
        Create.Table("user_customizations")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("user_id").AsGuid()
            .WithColumn("type").AsString()
            .WithColumn("value").AsString()
            .WithColumn("updated_at").AsDateTime();

        Create.UniqueConstraint("uq_user_customization")
            .OnTable("user_customizations")
            .Columns("user_id", "type");

        Create.Index("idx_user_customizations_user")
            .OnTable("user_customizations")
            .OnColumn("user_id");
    }
}
