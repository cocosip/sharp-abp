using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoring.Obs
{
    public class DefaultObsFileNameCalculator : IObsFileNameCalculator, ITransientDependency
    {
        protected ICurrentTenant CurrentTenant { get; }

        public DefaultObsFileNameCalculator(ICurrentTenant currentTenant)
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
