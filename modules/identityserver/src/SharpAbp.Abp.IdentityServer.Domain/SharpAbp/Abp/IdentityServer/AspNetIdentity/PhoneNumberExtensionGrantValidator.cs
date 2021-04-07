using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.IdentityServer.AspNetIdentity
{
    public class PhoneNumberExtensionGrantValidator : IExtensionGrantValidator
    {
        public const string ExtensionGrantType = "PhoneNumber_Password";
        public string GrantType => ExtensionGrantType;
        protected IdentityUserManager UserManager { get; }
        protected IdentitySecurityLogManager IdentitySecurityLogManager { get; }
        public PhoneNumberExtensionGrantValidator()
        {

        }

        public virtual async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var phoneNumber = context.Request.Raw["phone_number"];
            if (phoneNumber.IsNullOrWhiteSpace())
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = "invalid_phone_number"
                };
                return;
            }




        }
    }
}
