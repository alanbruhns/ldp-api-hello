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

app.MapGet("/api/participant/events", (IAWSEventStore eventStore) =>
{
    var events = eventStore.GetAllEvents();
    return Results.Ok(events);
})
.WithName("GetAllAWSEvents")
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

app.MapDelete("/api/participant/events", (IAWSEventStore eventStore) =>
{
    eventStore.RemoveEvents();

    return Results.Ok();
})
.WithName("DeleteAWSEvents")
.WithOpenApi();


app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

internal record AWSEvent
{
    public string Account { get; set; } = string.Empty;
    public string DetailType { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public List<object> Resources { get; set; } = [];
    public string Source { get; set; } = string.Empty;
    public DateTime Time { get; set; }
    public string Version { get; set; } = string.Empty;
    public SubscriptionCancelledIntegrationEvent Detail { get; set; } = new();
}

internal record SubscriptionCancelledIntegrationEvent
{
    public Guid Id { get; set; }
    public Guid ParticipantProfileId { get; set; }
    public string SubscriptionId { get; set; } = string.Empty;
}

internal interface IAWSEventStore
{
    void AddEvent(AWSEvent awsEvent);
    List<AWSEvent> GetAllEvents();
    void RemoveEvents();

}

internal class AWSEventStore : IAWSEventStore
{
    private readonly List<AWSEvent> _events = [];

    public void AddEvent(AWSEvent awsEvent)
        => _events.Add(awsEvent);
    public List<AWSEvent> GetAllEvents()
        => _events;
    public void RemoveEvents()
        => _events.Clear();
}