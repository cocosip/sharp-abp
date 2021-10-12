using SharpAbp.Abp.Account.Localization;
using Volo.Abp.Emailing.Templates;
using Volo.Abp.Localization;
using Volo.Abp.TextTemplating;

namespace SharpAbp.Abp.Account.Emailing.Templates
{
    public class AccountEmailTemplateDefinitionProvider : TemplateDefinitionProvider
    {
        public override void Define(ITemplateDefinitionContext context)
        {
            context.Add(
                new TemplateDefinition(
                    AccountEmailTemplates.PasswordResetLink,
                    displayName: LocalizableString.Create<AccountResource>($"TextTemplate:{AccountEmailTemplates.PasswordResetLink}"),
                    layout: StandardEmailTemplates.Layout,
                    localizationResource: typeof(AccountResource)
                ).WithVirtualFilePath("/SharpAbp/Abp/Account/Emailing/Templates/PasswordResetLink.tpl", true)
            );
        }
    }
}
