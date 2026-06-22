import type { MailerLiteConfigContext } from "./mailerlite.config.context.js";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";

export const MAILERLITE_CONFIG_CONTEXT = new UmbContextToken<MailerLiteConfigContext>("mailerlite-config-context");
