using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Umbraco.Community.Forms.Workflows.Configuration;
using Umbraco.Community.Forms.Workflows.Models.MailerLite;
using Umbraco.Community.Forms.Workflows.Services;
using Umbraco.Extensions;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Providers.Models;
using Umbraco.Forms.Core.Services;

namespace Umbraco.Community.Forms.Workflows.Workflows;

public class MailerLiteWorkflow : WorkflowType
{
    private readonly IPlaceholderParsingService placeholderParsingService;
    private readonly IMailerLiteService mailerLiteService;
    private readonly ILogger<MailerLiteWorkflow> logger;
    private readonly CommunityOptions options;

    [Setting("Email List",
        Description = "Configure the MailerLite group(s) to add subscribers to. API token is required if not already set globally",
        View = "Umbraco.Forms.Community.Workflows.PropertyEditorUi.MailerLiteMailingList")]
    public string EmailListConfiguration { get; set; } = string.Empty;

    [Setting("Email",
        Description = "Select the field to map the subscriber email address to",
        View = "Forms.PropertyEditorUi.TextWithFieldPicker")]
    public string Email { get; set; } = string.Empty;

    [Setting("Fields",
        Description = "Map form fields to MailerLite subscriber fields.",
        View = "Forms.PropertyEditorUi.FieldMapper")]
    public string Fields { get; set; } = string.Empty;

    [Setting("Double Opt In",
        Description = "Require subscribers to confirm their subscription before being marked active",
        View = "Umb.PropertyEditorUi.Toggle")]
    public string DoubleOptIn { get; set; } = "false";

    public MailerLiteWorkflow(IPlaceholderParsingService placeholderParsingService, IMailerLiteService mailerLiteService, ILogger<MailerLiteWorkflow> logger, IOptions<CommunityOptions> options)
    {
        this.placeholderParsingService = placeholderParsingService;
        this.mailerLiteService = mailerLiteService;
        this.logger = logger;
        this.options = options.Value;

        Id = new Guid("F1E2D3C4-B5A6-4789-9C8D-7E6F5A4B3C2D");
        Name = "MailerLite Subscriber";
        Description = "Add form submitter to a MailerLite group";
        Icon = "icon-mailbox";
        Group = "Services";
    }

    public override List<Exception> ValidateSettings()
    {
        var exceptions = new List<Exception>();

        if (string.IsNullOrEmpty(EmailListConfiguration))
        {
            exceptions.Add(new ArgumentException("Email List is not configured"));
        }

        if (string.IsNullOrEmpty(Email))
        {
            exceptions.Add(new ArgumentException("Email field is not configured"));
        }

        return exceptions;
    }

    public override async Task<WorkflowExecutionStatus> ExecuteAsync(WorkflowExecutionContext context)
    {
        try
        {
            var listConfig = JsonSerializer.Deserialize<MailingListConfig>(EmailListConfiguration);
            if (listConfig is null || listConfig.ListIds is null || listConfig.ListIds.Length == 0)
            {
                logger.LogWarning("MailerLite workflow not configured properly - missing group configuration");
                return WorkflowExecutionStatus.NotConfigured;
            }

            var subscriber = MapFormDataToSubscriber(context);

            if (string.IsNullOrEmpty(subscriber.Email))
            {
                logger.LogWarning("No email address found in form submission");
                return WorkflowExecutionStatus.Failed;
            }

            subscriber.Groups = listConfig.ListIds;

            mailerLiteService.ConfigureApiToken(listConfig.Token);
            var success = await mailerLiteService.AddSubscriber(subscriber);

            return success ? WorkflowExecutionStatus.Completed : WorkflowExecutionStatus.Failed;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing MailerLite workflow");
            return WorkflowExecutionStatus.Failed;
        }
    }

    private Subscriber MapFormDataToSubscriber(WorkflowExecutionContext context)
    {
        var subscriber = new Subscriber();

        List<FieldMapping> mappings = [];

        if (!string.IsNullOrEmpty(Fields))
        {
            mappings = [.. JsonSerializer.Deserialize<IEnumerable<FieldMapping>>(Fields, FormsJsonSerializerOptions.Default) ?? []];
        }

        try
        {
            subscriber.Email = placeholderParsingService.ParsePlaceHolders(Email, false, context.Record);

            var fields = new Dictionary<string, object>();
            foreach (var mapping in mappings)
            {
                var fieldValue = GetMappedFieldValue(mapping, context);
                if (string.IsNullOrEmpty(fieldValue)) continue;

                fields[mapping.Alias] = fieldValue;
            }

            if (fields.Count > 0)
            {
                subscriber.Fields = fields;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error parsing field mappings configuration");
        }

        if (bool.TryParse(DoubleOptIn, out bool doubleOptIn))
        {
            subscriber.Status = doubleOptIn ? "unconfirmed" : "active";
        }

        return subscriber;
    }

    private string GetMappedFieldValue(FieldMapping mapping, WorkflowExecutionContext context)
    {
        if (!string.IsNullOrEmpty(mapping.StaticValue))
        {
            return placeholderParsingService.ParsePlaceHolders(mapping.StaticValue, false, context.Record);
        }
        else if (!string.IsNullOrEmpty(mapping.Value))
        {
            var recordField = context.Record.GetRecordField(new Guid(mapping.Value));
            if (recordField != null)
            {
                return recordField.ValuesAsString(false);
            }
            else
                logger.LogWarning("Workflow {workflowName}: The field mapping with alias, {fieldMappingAlias}, did not match any record fields. This is probably caused by the record field being marked as sensitive and the workflow has been set not to include sensitive data", Workflow?.Name, mapping.Alias);
        }

        return string.Empty;
    }
}
