using System.Text.Json.Serialization;

namespace Umbraco.Community.Forms.Workflows.Models.MailerLite;

public class Group
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
