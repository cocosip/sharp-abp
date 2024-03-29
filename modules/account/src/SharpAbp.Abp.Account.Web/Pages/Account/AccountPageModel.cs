﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.Account.Localization;
using Volo.Abp;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Identity;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace SharpAbp.Abp.Account.Web.Pages.Account
{
    public abstract class AccountPageModel : AbpPageModel
    {
        public IAccountAppService AccountAppService { get; set; }
        public SignInManager<IdentityUser> SignInManager { get; set; }
        public IdentityUserManager UserManager { get; set; }
        public IdentitySecurityLogManager IdentitySecurityLogManager { get; set; }
        public IOptions<IdentityOptions> IdentityOptions { get; set; }
        public IExceptionToErrorInfoConverter ExceptionToErrorInfoConverter { get; set; }

        protected AccountPageModel()
        {
            LocalizationResourceType = typeof(AccountResource);
            ObjectMapperContext = typeof(AccountWebModule);
        }

        protected virtual void CheckCurrentTenant(Guid? tenantId)
        {
            if (CurrentTenant.Id != tenantId)
            {
                throw new ApplicationException($"Current tenant is different than given tenant. CurrentTenant.Id: {CurrentTenant.Id}, given tenantId: {tenantId}");
            }
        }

        protected virtual void CheckIdentityErrors(IdentityResult identityResult)
        {
            if (!identityResult.Succeeded)
            {
                throw new UserFriendlyException("Operation failed: " + identityResult.Errors.Select(e => $"[{e.Code}] {e.Description}").JoinAsString(", "));
            }

            //identityResult.CheckErrors(LocalizationManager); //TODO: Get from old Abp
        }

        protected virtual string GetLocalizeExceptionMessage(Exception exception)
        {
            if (exception is ILocalizeErrorMessage || exception is IHasErrorCode)
            {
                return ExceptionToErrorInfoConverter.Convert(exception, null).Message;
                //return ExceptionToErrorInfoConverter.Convert(exception, false).Message;
            }

            return exception.Message;
        }
    }
}
