import { UmbRepositoryBase } from "@umbraco-cms/backoffice/repository";
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { Config } from "../generated";

export class MailerLiteConfigRepository extends UmbRepositoryBase {
  constructor(host: UmbControllerHost) {
    super(host);
  }

  async groups(token?: string) {

    const { data, error } = await tryExecute(
      this,
      Config.getMailerLiteGroups({
        query: {
          token: token
        }
      })
    );

    if (data) {
      return { data };
    }

    return { error };
  }
}

export { MailerLiteConfigRepository as api };
