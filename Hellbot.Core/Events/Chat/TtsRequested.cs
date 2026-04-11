using System;
using System.Collections.Generic;
using System.Text;

namespace Hellbot.Core.Events.Chat
{

    public record TtsRequestPayload
    {
        public required string Text { get; init; }
        public required string VoiceId { get; init; }
        public int Priority { get; init; } = 0; 
    }

    public record TtsRequested : HellbotEvent<TtsRequestPayload>;

}
