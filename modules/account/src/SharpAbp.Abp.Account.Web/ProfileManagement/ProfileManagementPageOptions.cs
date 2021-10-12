﻿using System.Collections.Generic;

 namespace SharpAbp.Abp.Account.Web.ProfileManagement
{
    public class ProfileManagementPageOptions
    {
        public List<IProfileManagementPageContributor> Contributors { get; }

        public ProfileManagementPageOptions()
        {
            Contributors = new List<IProfileManagementPageContributor>();
        }
    }
}
