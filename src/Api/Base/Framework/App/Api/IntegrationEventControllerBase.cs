using Hello.Base.Framework.Core.IntegrationEvents;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hello.Base.Framework.App.Api
{
    [Route("/api/integration-events")]
    public abstract class IntegrationEventControllerBase : ControllerBase
    {
        private readonly IAWSEventStore _eventStore;
        private readonly IMediator _mediator;
        private readonly IEventToNotificationMapper _eventMapper;

        protected IntegrationEventControllerBase(IAWSEventStore eventStore, IMediator mediator, IEventToNotificationMapper eventMapper)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _eventMapper = eventMapper ?? throw new ArgumentNullException(nameof(eventMapper));
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProcessEvent([FromBody] AWSEvent awsEvent)
        {
            try
            {
                var notification = _eventMapper.Map(awsEvent);
                var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
                var handler = HttpContext.RequestServices.GetService(handlerType);

                if (handler == null)
                    return BadRequest($"No handler registered for event type: {awsEvent.DetailType}");

                await _mediator.Publish(notification);

                _eventStore.Enqueue(awsEvent);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to process event: {ex.Message}");
            }
        }

        [HttpGet]
        //[Authorize(AuthenticationSchemes = "ApiKey")]
        public IActionResult GetEvents()
        {
            var @event = _eventStore.TryDequeue();

            return Ok(@event);
        }
    }
}
