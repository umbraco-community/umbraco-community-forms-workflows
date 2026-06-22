using System.Text.Json.Serialization;

namespace Umbraco.Community.Forms.Workflows.Models.MailerLite;

public class Subscriber
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("fields")]
    public Dictionary<string, object>? Fields { get; set; }

    [JsonPropertyName("groups")]
    public string[]? Groups { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = "active";
}
