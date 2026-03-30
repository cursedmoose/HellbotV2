namespace Hellbot.Core.Events.Test
{
    public class TestMessageEvent : TestEvent
    {
        public string Message { get; init; } = default!;
    }
}
