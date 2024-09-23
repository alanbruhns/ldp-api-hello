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

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHealthChecks("/healthz"); // Liveness probe endpoint
app.UseHealthChecks("/readyz");  // Readiness probe endpoint

app.MapGet("/hello", () =>
{
    return "Hello";
})
.WithName("GetHello")
.WithOpenApi();


// POST endpoint for AWSEvent
app.MapPost("/api/participant/cancel", (AWSEvent awsEvent) =>
{
    // Here you can add your logic to process the AWSEvent
    return Results.Ok(awsEvent); // Return the event data for now
})
.WithName("PostAWSEvent")
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
    public Guid ParticipantId { get; set; }
}