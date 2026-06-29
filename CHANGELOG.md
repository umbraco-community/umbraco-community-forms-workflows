# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [18.0.0] - 2026-06-29

### Changed
- Upgraded to support Umbraco 18 and Umbraco Forms 18 (`Umbraco.Forms.Core.Providers` `[18.0.0,19)`)
- Migrated the management API OpenAPI registration from Swashbuckle to `Microsoft.AspNetCore.OpenApi`, using `AddBackOfficeOpenApiDocument` with back-office authentication
- Updated the backoffice client to `@umbraco-cms/backoffice` 18 (`lit` 3.3.1)
- Backoffice OpenAPI document is now served at `/umbraco/openapi/{name}.json` (was `/umbraco/swagger/{name}/swagger.json`)

### Removed
- Removed the obsolete Umbraco 17 test project; `Forms.Testv18` is now the sole test harness

## [17.0.2] - 2026-06-18

### Added
- MailerLite workflow for Umbraco Forms
  - Adds form subscribers to one or more MailerLite groups via a multi-select group picker
  - FieldMapper support for mapping form fields (and static values) to MailerLite subscriber fields
  - "Double Opt In" option to mark new subscribers as `unconfirmed` until confirmed (otherwise `active`)
  - Per-workflow API token override with fallback to `Community:Forms:MailerLite:ApiToken` in `appsettings.json`
  - Backoffice configuration UI with a token field and group selector populated from the MailerLite API

## [17.0.0]

### Added
- Migrated Mailcoach workflow to Umbraco 17
- Added Mailchimp and Campaign Monitor workflows

## [1.0.0] - 2024-07-29

### Added
- Initial implementation of Mailcoach workflow for Umbraco Forms
- Support for adding form subscribers to Mailcoach email lists
- Configuration-based API settings via appsettings.json for security
- Native Umbraco Forms workflow UI using `[Setting]` attributes:
  - Custom email list picker that loads available lists from Mailcoach API
  - FieldMapper for flexible field mapping (similar to Mailchimp workflow)
  - Support for form fields and static values
  - Direct Tags setting for applying tags to all subscribers
  - Option to skip email confirmation
  - Support for custom attributes via `extra_attributes`
- Support for Umbraco Forms 13.5+ and .NET 8
- Comprehensive error logging and handling
- Package targeting Umbraco CMS with Mailcoach integration

### Security
- API credentials stored in configuration instead of workflow settings
- Secure handling of API tokens via dependency injection

### Implementation
- Uses modern C# patterns with collection expressions
- Integrated with Umbraco's dependency injection container
- Follows Umbraco Forms workflow best practices

### Dependencies
- Umbraco.Forms.Core 13.5.0
- .NET 8.0 target framework