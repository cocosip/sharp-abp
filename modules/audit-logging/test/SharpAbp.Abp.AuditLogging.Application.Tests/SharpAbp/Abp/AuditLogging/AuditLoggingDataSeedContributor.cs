using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.AuditLogging;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Users;

namespace SharpAbp.Abp.AuditLogging
{
    public class AuditLoggingDataSeedContributor : ITransientDependency
    {
        protected IAuditingHelper AuditingHelper { get; }
        protected IAuditingManager AuditingManager { get; }
        protected ICurrentUser CurrentUser { get; }

        public AuditLoggingDataSeedContributor(
            IAuditingHelper auditingHelper,
            IAuditingManager auditingManager,
            ICurrentUser currentUser)
        {
            AuditingHelper = auditingHelper;
            AuditingManager = auditingManager;
            CurrentUser = currentUser;
        }

        public async Task SeedAsync()
        {
            using (var saveHandle = AuditingManager.BeginScope())
            {
                var auditLog = new AuditLogInfo()
                {
                    ApplicationName = "App1",
                    ClientId = "client1",
                    ClientName = "client",
                    ClientIpAddress = "127.0.0.1",
                    HttpStatusCode = 200,
                    HttpMethod = "GET",
                    Comments = new List<string>(),
                    Actions = new List<AuditLogActionInfo>(),
                };

                var auditLogAction = AuditingHelper.CreateAuditLogAction(
                    auditLog,
                    typeof(AuditLogAppService),
                    typeof(AuditLogAppService).GetMethod("GetAsync"),
                    new object[] { Guid.Parse("025A520D-9FE3-4226-B89D-FA542C3F2D6C") }
                );

                auditLog.EntityChanges.Add(new EntityChangeInfo()
                {
                    ChangeTime = DateTime.Now
                });

                //[Guid("025A520D-9FE3-4226-B89D-FA542C3F2D6C")]

                auditLog.Exceptions.Add(new EntityNotFoundException(typeof(AuditLog)));
                auditLogAction.ExecutionDuration = Convert.ToInt32(5000);
                auditLog.Actions.Add(auditLogAction);

            

                await saveHandle.SaveAsync();
            }
        }

    }
}
