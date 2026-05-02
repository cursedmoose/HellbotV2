using Hellbot.Core.Events;
using Hellbot.Core.Events.Session;
using Hellbot.Service.Clients.Twitch;

namespace Hellbot.Service.EventBus.Handlers.Session
{
    public class GameStartedHandler(TwitchClient twitch, IEventBus bus, ILogger<GameStartedHandler> logger) : EventHandlerBase<GameStarted>
    {
        public override async Task Handle(GameStarted evt)
        {
            // TODO: Move Game name Resolution upstream
            var currentGame = evt.Data.Name;
            var twitchGame = await twitch.API.Games.GetGamesAsync(gameNames: [currentGame]);

            await bus.Publish(new UpdateChannel
            {
                Data = new UpdateChannelPayload
                {
                    GameId = twitchGame.Data[0].Name,
                },
                Source = EventSource.Internal with { Channel = "GameStartedHandler" }
            });

            return;
        }
    }
}
