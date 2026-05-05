using Hellbot.Core.Events.Session;
using Hellbot.Service.Sessions;

namespace Hellbot.Service.EventBus.Handlers.Session
{
    public class UpdateChannelHandler(IStreamSessionManager sessions) : EventHandlerBase<UpdateChannel>
    {
        public override Task Handle(UpdateChannel evt) =>
            sessions.UpdateChannelAsync(evt.Data.GameId, evt.Data.Title);
    }
}
