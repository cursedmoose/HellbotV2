using Hellbot.Core.Events.Session;
using Hellbot.Service.Clients.Twitch;

namespace Hellbot.Service.EventBus.Handlers.Session
{
    public class UpdateChannelHandler(TwitchClient twitch, ILogger<UpdateChannelHandler> logger) : EventHandlerBase<UpdateChannel>
    {
        public override async Task Handle(UpdateChannel evt)
        {
            await twitch.API.Channels.ModifyChannelInformationAsync("twitch.BroadcasterId", new() { 
                GameId = evt.Data.GameId,
                Title = evt.Data.Title
            });
            return;
        }
    }
}
