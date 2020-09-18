using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.FastDFS
{
    public class DefaultFastDFSFileNameCalculator : IFastDFSFileNameCalculator, ITransientDependency
    {
        public virtual string Calculate(FileProviderArgs args)
        {
            return args.FileId;
        }
    }
}
