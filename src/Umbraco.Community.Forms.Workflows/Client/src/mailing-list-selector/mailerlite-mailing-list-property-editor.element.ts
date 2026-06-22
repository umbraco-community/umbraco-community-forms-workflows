import {
  html,
  css,
  customElement,
  property,
  when,
  ifDefined,
} from "@umbraco-cms/backoffice/external/lit"
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api"
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element"
import type { UmbPropertyEditorUiElement } from "@umbraco-cms/backoffice/property-editor"
import type {
  UUIInputEvent,
  UUIBooleanInputEvent,
} from "@umbraco-cms/backoffice/external/uui"
import { UmbChangeEvent } from "@umbraco-cms/backoffice/event"
import { MAILERLITE_CONFIG_CONTEXT } from "../contexts/mailerlite.config.context.token"
import type { Group } from "../generated/types.gen"

export type MailerLiteMailingList = {
  token?: string
  listIds?: string[]
}

@customElement("mailerlite-mailing-list-property-editor")
export class MailerLiteMailingListPropertyEditorElement
  extends UmbElementMixin(UmbLitElement)
  implements UmbPropertyEditorUiElement
{
  #configContext?: typeof MAILERLITE_CONFIG_CONTEXT.TYPE

  #value = ""
  @property({ type: String })
  get value() {
    return this.#value
  }
  set value(value: string) {
    const oldVal = this.#value
    this.#value = value
    this.requestUpdate("value", oldVal)

    this.#initializeMailingListConfig(value)
  }

  #mailingListConfig: MailerLiteMailingList = {}

  #groups: Array<Group> = []

  constructor() {
    super()
    this.consumeContext(MAILERLITE_CONFIG_CONTEXT, (instance) => {
      this.#configContext = instance
      this.#fetchGroups()
    })
  }

  #initializeMailingListConfig(value?: string) {
    if (value && value.length > 0) {
      try {
        this.#mailingListConfig = JSON.parse(value)
      } catch {}
    }
  }

  async #fetchGroups() {
    if (this.#configContext) {
      this.#groups =
        (await this.#configContext.getGroups(this.#mailingListConfig.token)) ?? []
      this.requestUpdate()
    }
  }

  #onTokenChange(e: UUIInputEvent) {
    this.#mailingListConfig.token = e.target.value.toString()
    // We need to re-query the groups.
    this.#fetchGroups()
  }

  #onGroupToggle(e: UUIBooleanInputEvent, groupId: string) {
    const checked = e.target.checked
    const selected = new Set(this.#mailingListConfig.listIds ?? [])
    if (checked) {
      selected.add(groupId)
    } else {
      selected.delete(groupId)
    }
    this.#mailingListConfig.listIds = [...selected]
    this.#refreshValue()
  }

  #refreshValue() {
    this.value = JSON.stringify(this.#mailingListConfig)
    this.dispatchEvent(new UmbChangeEvent())
  }

  static styles = css`
    :host {
      display: block;
    }
    .umb-forms-mailing-list {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }
    .umb-forms-mailing-list-field {
      display: flex;
      flex-direction: column;
      margin-bottom: 0.25rem;
      gap: 0.25rem;
    }
  `

  render() {
    return html` <div class="umb-forms-mailing-list">
      <div class="umb-forms-mailing-list-field">
        <uui-label for="token">API Token</uui-label>
        <uui-input
          type="text"
          id="token"
          label="Token"
          auto-width=""
          placeholder="MailerLite API Token"
          .value=${ifDefined(this.#mailingListConfig.token)}
          @change=${(e: UUIInputEvent) => this.#onTokenChange(e)}>
        </uui-input>
      </div>
      <div class="umb-forms-mailing-list-field">
        ${when(
          this.#groups.length > 0,
          () =>
            html`
              <uui-label>Groups</uui-label>
              ${this.#groups.map(
                (g) => html`
                  <uui-checkbox
                    label=${g.name}
                    .value=${g.id}
                    ?checked=${this.#mailingListConfig.listIds?.includes(g.id) ??
                    false}
                    @change=${(e: UUIBooleanInputEvent) =>
                      this.#onGroupToggle(e, g.id)}>
                    ${g.name}
                  </uui-checkbox>
                `
              )}
            `,
          () =>
            html`<p>
              A valid API Token must be configured to display available Groups
            </p>`
        )}
      </div>
    </div>`
  }
}

export default MailerLiteMailingListPropertyEditorElement

declare global {
  interface HTMLElementTagNameMap {
    "mailerlite-mailing-list-property-editor": MailerLiteMailingListPropertyEditorElement
  }
}
