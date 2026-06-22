using System.Text.Json.Serialization;

namespace Umbraco.Community.Forms.Workflows.Models.MailerLite;

public class MailingListConfig
{
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    [JsonPropertyName("listIds")]
    public string[]? ListIds { get; set; }
}
