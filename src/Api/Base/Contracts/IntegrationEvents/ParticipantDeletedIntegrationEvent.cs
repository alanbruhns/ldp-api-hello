using MediatR;

namespace Hello.Base.Contracts.IntegrationEvents
{
    public class ParticipantDeletedIntegrationEvent : INotification
    {
        public Guid Id { get; set; }
        public Guid ParticipantProfileId { get; set; }
        public string SubscriptionId { get; set; } = string.Empty;
    }
}
