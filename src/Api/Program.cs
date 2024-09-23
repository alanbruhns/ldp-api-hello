using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "wwwroot",
    ApplicationName = typeof(Program).Assembly.FullName,
    EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
    ContentRootPath = Directory.GetCurrentDirectory(),
});

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks(); // Add health check services
builder.Services.AddLogging();
builder.Services.AddSingleton<IAWSEventStore, AWSEventStore>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHealthChecks("/healthz"); // Liveness probe endpoint
app.UseHealthChecks("/readyz");  // Readiness probe endpoint

app.MapGet("/hello", (ILogger<Program> logger) =>
{
    logger.LogInformation("Hello endpoint was called.");
    return "Hello";
})
.WithName("GetHello")
.WithOpenApi();


// POST endpoint for AWSEvent
app.MapPost("/api/participant/cancel", (AWSEvent awsEvent, IAWSEventStore eventStore, ILogger<Program> logger) =>
{
    logger.LogInformation("Received an AWS Event for Participant cancellation. Id: {Id}, ParticipantId: {ParticipantProfileId}", awsEvent.Detail.Id, awsEvent.Detail.ParticipantProfileId);

    eventStore.AddEvent(awsEvent);

    return Results.Ok(awsEvent); 
})
.WithName("PostAWSEvent")
.WithOpenApi();

app.MapGet("/api/participant/events", (IAWSEventStore eventStore) =>
{
    var events = eventStore.GetAllEvents();
    return Results.Ok(events);
})
.WithName("GetAllAWSEvents")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

internal record AWSEvent
{
    public string Account { get; set; }
    public string DetailType { get; set; }
    public string Id { get; set; }
    public string Region { get; set; }
    public List<object> Resources { get; set; }
    public string Source { get; set; }
    public DateTime Time { get; set; }
    public string Version { get; set; }
    public SubscriptionCancelledIntegrationEvent Detail { get; set; }
}

internal record SubscriptionCancelledIntegrationEvent
{
    public Guid Id { get; set; }
    public Guid ParticipantProfileId { get; set; }
}

internal interface IAWSEventStore
{
    void AddEvent(AWSEvent awsEvent);
    List<AWSEvent> GetAllEvents();
}

internal class AWSEventStore : IAWSEventStore
{
    private readonly List<AWSEvent> _events = new();

    public void AddEvent(AWSEvent awsEvent)
    {
        _events.Add(awsEvent);
    }

    public List<AWSEvent> GetAllEvents()
    {
        return _events;
    }
}