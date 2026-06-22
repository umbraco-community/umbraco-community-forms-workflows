using System.Text.Json.Serialization;

namespace Umbraco.Community.Forms.Workflows.Models.MailerLite;

public class GroupsResponse
{
    [JsonPropertyName("data")]
    public List<Group> Data { get; set; } = [];

    [JsonPropertyName("links")]
    public Links? Links { get; set; }

    [JsonPropertyName("meta")]
    public Meta? Meta { get; set; }
}
