using Hellbot.Core.Events;
using Hellbot.Core.Events.Chat;
using Hellbot.Service.Clients.Twitch;

namespace Hellbot.Service.EventBus.Handlers.Chat
{
    public class SendChatHandler(TwitchClient _twitch, ILogger<SendChatHandler> logger) : EventHandlerBase<SendChatMessage>
    {
        public override async Task Handle(SendChatMessage evt)
        {
            var message = evt.Data.Message;
            var platform = evt.Data.Channel.Platform;
            var replyTo = string.IsNullOrEmpty(evt.Data.ReplyTo) ? null : evt.Data.ReplyTo;
            switch (platform) {
                case PlatformSource.Twitch:
                    await _twitch.SendMessageAsync(message, replyTo);
                    return;
                case PlatformSource.Test:
                case PlatformSource.API:
                    logger.LogInformation("{Message}", message);
                    return;
            }
        }
    }
}
