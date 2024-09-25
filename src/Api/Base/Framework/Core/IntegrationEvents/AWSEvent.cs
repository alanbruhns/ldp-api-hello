namespace Hello.Base.Framework.Core.IntegrationEvents
{
    public record AWSEvent
    {
        public string Account { get; set; } = string.Empty;
        public string DetailType { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public List<object> Resources { get; set; } = [];
        public string Source { get; set; } = string.Empty;
        public DateTime Time { get; set; }
        public string Version { get; set; } = string.Empty;
        public object Detail { get; set; } // Maps to a class from Base.Contracts.IntegrationEvents
    }
}
