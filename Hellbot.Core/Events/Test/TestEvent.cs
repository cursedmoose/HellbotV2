namespace Hellbot.Core.Events.Test
{
    public record TestEvent : HellbotEvent<TestPayload>
    {
        public TestEvent() {
            Source = EventSource.Test;
        }
    }

    public record TestPayload()
    {
        public required string Message;
    }
}
