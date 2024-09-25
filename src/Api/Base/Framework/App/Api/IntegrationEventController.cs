using Hello.Base.Framework.Core.IntegrationEvents;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hello.Base.Framework.App.Api
{
    [Route("/api/integration-events")]
    public class IntegrationEventController : IntegrationEventControllerBase
    {
        public IntegrationEventController(IMediator mediator, IEventToNotificationMapper eventMapper, IAWSEventStore eventStore)
            : base(eventStore, mediator, eventMapper)
        {
        }
    }
}
