using MediatR;

namespace Hello.Base.Framework.Core.IntegrationEvents
{
    public abstract class IntegrationEventHandler<TEvent> : INotificationHandler<TEvent> where TEvent : INotification
    {
        public abstract Task Handle(TEvent notification, CancellationToken cancellationToken);
    }
}
