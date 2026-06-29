using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Api.Management.OpenApi;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Community.Forms.Workflows.Configuration;
using Umbraco.Community.Forms.Workflows.Services;
using Umbraco.Community.Forms.Workflows.Workflows;
using Umbraco.Forms.Core.Providers;

namespace Umbraco.Community.Forms.Workflows.Composing;

public class Composer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        // Umbraco 18 replaced Swashbuckle with Microsoft.AspNetCore.OpenApi.
        // Register the management API document with the back-office conventions and authentication.
        // Endpoints are included automatically via the [MapToApi(Constants.ApiName)] attribute on the controllers.
        builder.AddBackOfficeOpenApiDocument(
            Constants.ApiName,
            document => document
                .WithTitle("Umbraco Community Forms Workflow Management Api")
                .WithBackOfficeAuthentication()
                .WithJsonOptions(Umbraco.Cms.Core.Constants.JsonOptionsNames.BackOffice)
                .ConfigureOpenApiOptions(options =>
                    options.AddDocumentTransformer((doc, _, _) =>
                    {
                        doc.Info.Version = "Latest";
                        doc.Info.Description = "Api access for Umbraco Community Forms Workflow Management operations";
                        return Task.CompletedTask;
                    })));

        builder.Services.Configure<CommunityOptions>(builder.Config.GetSection(CommunityOptions.SectionName));
        builder.Services.AddHttpClient<IMailcoachService, MailcoachService>();
        builder.Services.AddHttpClient<IMailerLiteService, MailerLiteService>();

        builder.WithCollectionBuilder<WorkflowCollectionBuilder>()
            .Add<CampaignMonitorWorkflow>()
            .Add<MailcoachWorkflow>()
            .Add<MailChimpWorkflow>()
            .Add<MailerLiteWorkflow>();

    }
}
