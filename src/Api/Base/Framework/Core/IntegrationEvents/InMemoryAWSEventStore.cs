namespace Hello.Base.Framework.Core.IntegrationEvents
{
    public interface IAWSEventStore
    {
        void Enqueue(AWSEvent awsEvent);
        AWSEvent? TryPeek();
        void Clear();
    }

    public class InMemoryAWSEventStore : IAWSEventStore
    {
        private readonly Queue<AWSEvent> _events = [];

        public void Enqueue(AWSEvent awsEvent)
            => _events.Enqueue(awsEvent);

        public AWSEvent? TryPeek()
            => _events.TryPeek(out var @event)
                ? @event 
                : null;

        public void Clear()
            => _events.Clear();
    }
}
