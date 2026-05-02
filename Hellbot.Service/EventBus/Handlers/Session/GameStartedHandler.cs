using Hellbot.Core.Events.Session;
using Hellbot.Service.Clients.Twitch;

namespace Hellbot.Service.EventBus.Handlers.Session
{
    public class GameStartedHandler(TwitchClient twitch, ILogger<GameStartedHandler> logger) : EventHandlerBase<GameStarted>
    {
        public override async Task Handle(GameStarted evt)
        {
            // TODO: Make this fire a UpdateTitle and/or UpdateGame request
            var currentGame = evt.Data.Name;
            var twitchGame = await twitch.API.Games.GetGamesAsync(gameNames: [currentGame]);
            await twitch.API.Channels.ModifyChannelInformationAsync("twitch.BroadcasterId", new() { GameId = twitchGame.Data[0].Id });
            return;
        }
    }
}
