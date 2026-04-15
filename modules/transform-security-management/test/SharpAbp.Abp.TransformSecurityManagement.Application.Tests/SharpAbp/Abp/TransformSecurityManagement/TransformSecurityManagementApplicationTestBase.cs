using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Testing;
using Volo.Abp.Uow;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    public abstract class TransformSecurityManagementApplicationTestBase : AbpIntegratedTest<TransformSecurityManagementApplicationTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }

        protected async Task WithUnitOfWorkAsync(Func<Task> action)
        {
            var unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();

            using var unitOfWork = unitOfWorkManager.Begin();
            await action();
            await unitOfWork.CompleteAsync();
        }

        protected async Task<T> WithUnitOfWorkAsync<T>(Func<Task<T>> action)
        {
            var unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();

            using var unitOfWork = unitOfWorkManager.Begin();
            var result = await action();
            await unitOfWork.CompleteAsync();
            return result;
        }
    }
}
