using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoring.Aws
{
    public class DefaultAwsFileNameCalculator : IAwsFileNameCalculator, ITransientDependency
    {
        protected ICurrentTenant CurrentTenant { get; }
        public DefaultAwsFileNameCalculator(ICurrentTenant currentTenant)
        {
            CurrentTenant = currentTenant;
        }

        public virtual string Calculate(FileProviderArgs args)
        {
            return CurrentTenant.Id == null
                ? $"host/{args.FileId}"
                : $"tenants/{CurrentTenant.Id.Value:D}/{args.FileId}";
        }

    }
}
