using Hellbot.Core.Events.Rewards;
using Hellbot.Service.Data.Tables;
using Hellbot.Service.Data.Tables.Users;
using Hellbot.Service.Users;
using Microsoft.Data.Sqlite;

namespace Hellbot.Service.EventBus.Handlers.Rewards;

public class GrantRewardHandler(
    UserEntitlementsTable entitlements,
    EntitlementCatalogTable catalog,
    IUserService userService,
    ILogger<GrantRewardHandler> logger) : EventHandlerBase<GrantReward>
{
    private const int SqliteConstraint = 19; // SQLITE_CONSTRAINT

    public async override Task Handle(GrantReward evt)
    {
        var catalogItemId = evt.Data.EntitlementCatalogItemId;
        var catalogItem = await catalog.GetById(catalogItemId);
        if (catalogItem is null)
        {
            logger.LogWarning(
                "Could not grant catalog item {CatalogItemId}: not found.",
                catalogItemId);
            return;
        }

        if (!catalogItem.IsActive)
        {
            logger.LogWarning(
                "Could not grant catalog item {CatalogItemId}: inactive in catalog.",
                catalogItemId);
            return;
        }

        var rewardReceiver = await userService.GetUserId(evt.Data.Receiver);
        if (rewardReceiver is not Guid userId)
        {
            logger.LogWarning(
                "Could not grant catalog item {CatalogItemId} to user={User} as they did not exist!",
                catalogItemId,
                evt.Data.Receiver);
            return;
        }

        try
        {
            await entitlements.Grant(userId, catalogItemId);
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == SqliteConstraint)
        {
            logger.LogWarning(
                "Duplicate grant skipped: user={UserId} already has catalog item {CatalogItemId}.",
                userId,
                catalogItemId);
        }

        return;
    }
}
