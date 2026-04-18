using Hellbot.Core.Events.Rewards;
using Hellbot.Service.Data.Tables.Users;
using Hellbot.Service.Users;

namespace Hellbot.Service.EventBus.Handlers.Rewards
{
    public class GrantRewardHandler(UserEntitlementsTable db, IUserService userService, ILogger<GrantRewardHandler> logger) : EventHandlerBase<GrantReward>
    {
        public async override Task Handle(GrantReward evt)
        {
            var rewardReceiver = await userService.GetOrCreateUser(evt.Data.Receiver);
            await db.Create(rewardReceiver.Id, evt.Data.Reward);

            return;
        }
    }
}
