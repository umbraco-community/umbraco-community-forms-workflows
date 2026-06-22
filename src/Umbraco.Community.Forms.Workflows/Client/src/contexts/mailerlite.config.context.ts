import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { UmbContextBase } from "@umbraco-cms/backoffice/class-api";
import { MailerLiteConfigRepository } from "../repository/mailerlite.config.repository.js";
import type { Group } from "../generated/types.gen.js";
import { MAILERLITE_CONFIG_CONTEXT } from "./mailerlite.config.context.token.js";

export class MailerLiteConfigContext extends UmbContextBase {

  #repository: MailerLiteConfigRepository;
  #groups?: Group[];
  #token?: string = undefined;

  constructor(host: UmbControllerHost) {
    super(host, MAILERLITE_CONFIG_CONTEXT);
    this.#repository = new MailerLiteConfigRepository(host);
  }

  async getGroups(token?: string): Promise<Group[] | undefined> {
    if (!this.#groups || this.#token !== token) {
      this.#token = token;
      const result = await this.#repository.groups(token);
      if (result.data) {
        this.#groups = result.data;
      }
    }
    return this.#groups;
  }
}

export default MailerLiteConfigContext;
