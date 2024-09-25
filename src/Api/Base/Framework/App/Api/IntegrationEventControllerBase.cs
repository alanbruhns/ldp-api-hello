using Hello.Base.Framework.Core.IntegrationEvents;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hello.Base.Framework.App.Api
{
    [Route("/api/integration-events")]
    public abstract class IntegrationEventControllerBase : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IEventToNotificationMapper _eventMapper;

        protected IntegrationEventControllerBase(IMediator mediator, IEventToNotificationMapper eventMapper)
        {
            _mediator = mediator;
            _eventMapper = eventMapper;
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

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to process event: {ex.Message}");
            }
        }
    }
}
