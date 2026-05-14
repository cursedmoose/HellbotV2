using Hellbot.Core.Events;
using Hellbot.Core.Events.Rewards;

namespace Hellbot.Service.EventBus.Twitch;

/// <summary>
/// Maps Twitch channel points custom reward redemptions to concrete <see cref="TwitchRewardRedeemed"/> leaf types.
/// Replace placeholder reward ids with values from Creator Dashboard or Helix.
/// </summary>
public sealed class TwitchChannelPointsRedemptionInterpreter(IEventBus bus, ILogger<TwitchChannelPointsRedemptionInterpreter> logger)
{
    private const string StartRumorRewardId = "__HellbotPlaceholder_StartRumor__";
    private const string LoseSweetrollsRewardId = "__HellbotPlaceholder_LoseSweetrolls__";
    private const string RequestTtsVoiceRewardId = "__HellbotPlaceholder_RequestTtsVoice__";
    private const string StopStreamRewardId = "__HellbotPlaceholder_StopStream__";

    public Task InterpretAsync(TwitchChannelPointsRedemption redemption, EventContext context)
    {
        var payload = new RewardRedeemedPayload
        {
            Cost = redemption.Cost,
            RewardId = redemption.RewardId,
            RewardTitle = redemption.Title,
            RedemptionId = redemption.RedemptionId,
            UserInput = redemption.UserInput,
            CorrelationId = null,
        };

        RewardRedeemed? evt = redemption.RewardId switch
        {
            StartRumorRewardId => new StartRumorRewardRedeemed { Source = EventSource.Twitch, Context = context, Data = payload },
            LoseSweetrollsRewardId => new LoseSweetrollsRewardRedeemed { Source = EventSource.Twitch, Context = context, Data = payload },
            RequestTtsVoiceRewardId => new RequestTtsVoiceRewardRedeemed { Source = EventSource.Twitch, Context = context, Data = payload },
            StopStreamRewardId => new StopStreamRewardRedeemed { Source = EventSource.Twitch, Context = context, Data = payload },
            _ => null,
        };

        if (evt is null)
        {
            logger.LogInformation(
                "Ignoring channel points redemption for untracked reward {RewardId} ({Title}).",
                redemption.RewardId,
                redemption.Title);
            return Task.CompletedTask;
        }

        return bus.Publish(evt);
    }
}
