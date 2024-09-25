using Hello.Base.Contracts.IntegrationEvents;
using Hello.Base.Framework.Core.IntegrationEvents;

namespace Hello.IntegrationEvents
{
    public class ParticipantDeletedIntegrationEventHandler : IntegrationEventHandler<ParticipantDeletedIntegrationEvent>
    {
        public override Task Handle(ParticipantDeletedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Handling ParticipantDeletedIntegrationEvent event for ParticipantProfileId: {notification.ParticipantProfileId}");
            return Task.CompletedTask;
        }
    }

}
