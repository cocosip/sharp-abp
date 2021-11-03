using IdentityServer4.Validation;

namespace SharpAbp.Abp.IdentityServer.Extensions
{
    public static class ExtensionGrantValidationContextExtensions
    {
        public static string GetLoginProvider(this ExtensionGrantValidationContext context)
        {
            return context.Request?.Raw?[ExternalCredentialsParameterConstants.LoginProvider];
        }

        public static string GetProviderKey(this ExtensionGrantValidationContext context)
        {
            return context.Request?.Raw?[ExternalCredentialsParameterConstants.ProviderKey];
        }


    }
}
