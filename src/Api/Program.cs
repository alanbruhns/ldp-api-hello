using Hello.Base.Framework.Core.IntegrationEvents;

namespace Api
{
    public class SomethinNonSatic
    {

    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddApplicationPart(typeof(SomethinNonSatic).Assembly);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHealthChecks();
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies([typeof(SomethinNonSatic).Assembly]));

            builder.Services.AddIntegrationEventHandling(builder.Configuration);

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHealthChecks("/healthz");
            app.UseHealthChecks("/readyz");

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
