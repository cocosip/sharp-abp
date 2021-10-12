﻿using System.Threading.Tasks;

 namespace SharpAbp.Abp.Account.Web.ProfileManagement
{
    public interface IProfileManagementPageContributor
    {
        Task ConfigureAsync(ProfileManagementPageCreationContext context);
    }
}
