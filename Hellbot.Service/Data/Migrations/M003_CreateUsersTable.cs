using FluentMigrator;
using Microsoft.AspNetCore.Http.HttpResults;
using TwitchLib.EventSub.Core.Models.Moderate;

namespace Hellbot.Service.Data.Migrations
{
    [Migration(2026041201)]
    public class M003_CreateUsersTable : Migration
    {
        public override void Up()
        {
            Create.Table("users")
                .WithColumn("id").AsGuid().PrimaryKey()
                .WithColumn("twitchId").AsString();
        }

        public override void Down()
        {
            Delete.Table("users");
        }
    }
}
