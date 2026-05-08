using FluentMigrator;

namespace Hellbot.Service.Data.Migrations;

[Migration(2026050701)]
public class M008_CreateEntitlementCatalog : Migration
{
    public override void Up()
    {
        Create.Table("entitlement_catalog")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("entitlement_type").AsString().NotNullable()
            .WithColumn("entitlement_id").AsString().NotNullable()
            .WithColumn("is_active").AsBoolean().NotNullable().WithDefaultValue(true);

        Create.UniqueConstraint("uq_entitlement_catalog_type_id")
            .OnTable("entitlement_catalog")
            .Columns("entitlement_type", "entitlement_id");
    }

    public override void Down()
    {
        Delete.Table("entitlement_catalog");
    }
}
