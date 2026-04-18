using Hellbot.Core.Events.Rewards;
using Hellbot.Service.Data.Tables.Users;
using Hellbot.Service.Users;

namespace Hellbot.Service.EventBus.Handlers.Rewards
{
    public class GrantRewardHandler(UserEntitlementsTable db, IUserService userService, ILogger<GrantRewardHandler> logger) : EventHandlerBase<GrantReward>
    {
        public async override Task Handle(GrantReward evt)
        {
            var rewardReceiver = await userService.GetUserId(evt.Data.Receiver);
            if (rewardReceiver is Guid userId)
            {
                await db.Create(userId, evt.Data.Reward);
            }
            else
            {
                logger.LogWarning("Could not grant user={User} a reward={Reward} as they did not exist!", evt.Data.Receiver, evt.Data.Reward.Type);
            }

            return;
        }
    }
}
