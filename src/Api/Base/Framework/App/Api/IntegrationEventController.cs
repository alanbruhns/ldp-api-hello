using Hello.Base.Framework.Core.IntegrationEvents;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hello.Base.Framework.App.Api
{
    [Route("/api/integration-events")]
    public class IntegrationEventController : IntegrationEventControllerBase
    {
        private readonly IAWSEventStore _eventStore;

        public IntegrationEventController(IMediator mediator, IEventToNotificationMapper eventMapper, IAWSEventStore eventStore)
            : base(mediator, eventMapper)
        {
            _eventStore = eventStore;
        }

        public override async Task<IActionResult> ProcessEvent([FromBody] AWSEvent awsEvent)
        {
            var result = await base.ProcessEvent(awsEvent);

            _eventStore.Enqueue(awsEvent);

            return result;
        }

        [HttpGet]
        //[Authorize(AuthenticationSchemes = "ApiKey")]
        public IActionResult GetEvents()
        {
            var @event = _eventStore.TryPeek();

            return Ok(@event);
        }
    }
}
