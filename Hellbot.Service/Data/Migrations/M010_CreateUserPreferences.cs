using FluentMigrator;

namespace Hellbot.Service.Data.Migrations;

[Migration(2026050802)]
public class M010_CreateUserPreferences : Migration
{
    public override void Up()
    {
        Create.Table("user_preferences")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("user_id").AsGuid().NotNullable()
            .WithColumn("entitlement_type").AsString().NotNullable()
            .WithColumn("selected_entitlement_catalog_id").AsGuid().NotNullable();

        Create.ForeignKey("fk_user_preferences_users")
            .FromTable("user_preferences").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("id");

        Create.ForeignKey("fk_user_preferences_entitlement_catalog")
            .FromTable("user_preferences").ForeignColumn("selected_entitlement_catalog_id")
            .ToTable("entitlement_catalog").PrimaryColumn("id");

        Create.UniqueConstraint("uq_user_preferences_user_type")
            .OnTable("user_preferences")
            .Columns("user_id", "entitlement_type");

        Create.Index("idx_user_preferences_user")
            .OnTable("user_preferences")
            .OnColumn("user_id");
    }

    public override void Down()
    {
        Delete.Table("user_preferences");
    }
}
