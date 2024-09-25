using MediatR;
using Newtonsoft.Json;

namespace Hello.Base.Framework.Core.IntegrationEvents
{
    public interface IEventToNotificationMapper
    {
        INotification Map(AWSEvent integrationEvent);
    }

    public class EventToNotificationMapper : IEventToNotificationMapper
    {
        private readonly IServiceProvider _serviceProvider;

        public EventToNotificationMapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public INotification? Map(AWSEvent awsEvent)
        {
            var notificationType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(t => t.Name == awsEvent.DetailType && typeof(INotification).IsAssignableFrom(t));

            if (notificationType == null)
            {
                throw new InvalidOperationException($"No INotificationHandlers found for the Inotification of Type: '{awsEvent.DetailType}'.");
            }

            var notification = JsonConvert.DeserializeObject(awsEvent.Detail.ToString(), notificationType);
            return (INotification)notification;
        }
    }
}
