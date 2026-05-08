using FluentMigrator;

namespace Hellbot.Service.Data.Migrations;

[Migration(2026050801)]
public class M009_RebuildUserEntitlementsForCatalog : Migration
{
    public override void Up()
    {
        Delete.Table("user_entitlements");

        Create.Table("user_entitlements")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("user_id").AsGuid().NotNullable()
            .WithColumn("entitlement_catalog_id").AsGuid().NotNullable()
            .WithColumn("earned_at").AsDateTime().NotNullable();

        Create.UniqueConstraint("uq_user_entitlements_user_catalog")
            .OnTable("user_entitlements")
            .Columns("user_id", "entitlement_catalog_id");

        Create.Index("idx_user_entitlements_user")
            .OnTable("user_entitlements")
            .OnColumn("user_id");
    }

    public override void Down()
    {
        Delete.Table("user_entitlements");

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
}
