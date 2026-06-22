using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Community.Forms.Workflows.Configuration;
using Umbraco.Community.Forms.Workflows.Models.MailerLite;
using Umbraco.Extensions;

namespace Umbraco.Community.Forms.Workflows.Services;

public class MailerLiteService : IMailerLiteService
{
    private const string ApiBaseAddress = "https://connect.mailerlite.com/api/";

    private readonly CommunityOptions options;
    private readonly HttpClient httpClient;
    private readonly ILogger<MailerLiteService> logger;

    public MailerLiteService(
        HttpClient httpClient,
        ILogger<MailerLiteService> logger,
        IOptions<CommunityOptions> options)
    {
        this.httpClient = httpClient;
        this.logger = logger;
        this.options = options.Value;

        httpClient.BaseAddress = new Uri(ApiBaseAddress);
        ConfigureApiToken();
    }

    /// <inheritdoc />
    public async Task<bool> AddSubscriber(Subscriber subscriber)
    {
        var endpoint = "subscribers";

        var requestBody = JsonSerializer.Serialize(subscriber);

        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(endpoint, content);

        if (response.IsSuccessStatusCode)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Successfully added subscriber {email} to MailerLite", subscriber.Email);
            }
            return true;
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            logger.LogError("Failed to add subscriber {email} to MailerLite. Status: {StatusCode}, Response: {Response}",
                subscriber.Email, response.StatusCode, errorContent);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<List<Group>> GetGroupsAsync()
    {
        try
        {
            var endpoint = "groups?limit=1000";

            var response = await httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var groupsResponse = JsonSerializer.Deserialize<GroupsResponse>(content);

                if (groupsResponse?.Data != null)
                {
                    if (logger.IsEnabled(LogLevel.Debug))
                    {
                        logger.LogDebug("Successfully retrieved {count} groups from MailerLite", groupsResponse.Data.Count);
                    }
                    return groupsResponse.Data;
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                logger.LogError("Failed to retrieve groups from MailerLite. Status: {statusCode}, Response: {response}",
                    response.StatusCode, errorContent);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while retrieving groups from MailerLite");
        }

        return [];
    }

    /// <inheritdoc />
    public void ConfigureApiToken(string? token = null)
    {
        token ??= options.MailerLite?.ApiToken;
        if (token.IsNullOrWhiteSpace())
        {
            logger.LogWarning("ConfigureApiToken: MailerLite API not configured properly");
        }
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
