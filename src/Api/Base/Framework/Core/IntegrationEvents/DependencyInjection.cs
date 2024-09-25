namespace Hello.Base.Framework.Core.IntegrationEvents
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIntegrationEventHandling(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<IEventToNotificationMapper, EventToNotificationMapper>();

            services.AddSingleton<IAWSEventStore, InMemoryAWSEventStore>();

            return services;
        }
    }
}
