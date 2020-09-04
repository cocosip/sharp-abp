using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoring
{
    public class DefaultS3FileNameCalculator : IS3FileNameCalculator, ITransientDependency
    {
        protected ICurrentTenant CurrentTenant { get; }

        public DefaultS3FileNameCalculator(ICurrentTenant currentTenant)
        {
            CurrentTenant = currentTenant;
        }

        public virtual string Calculate(FileProviderArgs args)
        {
            return CurrentTenant.Id == null
                ? $"host/{args.FileName}"
                : $"tenants/{CurrentTenant.Id.Value:D}/{args.FileName}";
        }
    }
}
