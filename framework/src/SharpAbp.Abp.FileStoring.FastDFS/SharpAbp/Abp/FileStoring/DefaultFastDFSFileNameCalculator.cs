using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring
{
    public class DefaultFastDFSFileNameCalculator : IFastDFSFileNameCalculator, ITransientDependency
    {

        public DefaultFastDFSFileNameCalculator()
        {

        }

        public virtual string Calculate(FileProviderArgs args)
        {
            return args.FileId;
        }
    }
}
