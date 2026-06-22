using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Community.Forms.Workflows.Models.MailerLite;
using Umbraco.Community.Forms.Workflows.Services;

namespace Umbraco.Community.Forms.Workflows.Controllers;

[ApiExplorerSettings(GroupName = "Config")]
[ApiVersion("1.0")]
[Authorize(Policy = "SectionAccessForms")]
[MapToApi(Constants.ApiName)]
public class MailerLiteController(IMailerLiteService mailerLiteService) : ManagementApiControllerBase
{
    [HttpGet("MailerLiteGroups")]
    [ProducesResponseType(typeof(IEnumerable<Group>), 200)]
    public async Task<IEnumerable<Group>> GetMailerLiteGroups(string? token)
    {
        mailerLiteService.ConfigureApiToken(token);
        try
        {
            return await mailerLiteService.GetGroupsAsync();
        }
        catch
        {
            return [];
        }
    }
}
