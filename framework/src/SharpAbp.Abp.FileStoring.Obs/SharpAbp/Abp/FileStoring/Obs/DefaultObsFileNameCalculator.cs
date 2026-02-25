using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.Obs
{
    public class DefaultObsFileNameCalculator : IObsFileNameCalculator, ITransientDependency
    {
        protected IFilePathBuilder FilePathBuilder { get; }

        public DefaultObsFileNameCalculator(IFilePathBuilder filePathBuilder)
        {
            FilePathBuilder = filePathBuilder;
        }

        public virtual string Calculate(FileProviderArgs args)
        {
            return FilePathBuilder.Build(args);
        }
    }
}
