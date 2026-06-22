using Umbraco.Community.Forms.Workflows.Models.MailerLite;

namespace Umbraco.Community.Forms.Workflows.Services;

public interface IMailerLiteService
{
    /// <summary>
    /// Retrieve the available MailerLite groups
    /// </summary>
    Task<List<Group>> GetGroupsAsync();

    /// <summary>
    /// Create or update a subscriber in MailerLite
    /// </summary>
    /// <param name="subscriber"></param>
    Task<bool> AddSubscriber(Subscriber subscriber);

    /// <summary>
    /// Configure MailerLite to use the supplied token using Configuration as a fallback
    /// </summary>
    /// <param name="token"></param>
    void ConfigureApiToken(string? token = null);
}
