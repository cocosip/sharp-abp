using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.Azure
{
    public class DefaultAzureFileNameCalculator : IAzureFileNameCalculator, ITransientDependency
    {
        protected IFilePathBuilder FilePathBuilder { get; }

        public DefaultAzureFileNameCalculator(IFilePathBuilder filePathBuilder)
        {
            FilePathBuilder = filePathBuilder;
        }

        public virtual string Calculate(FileProviderArgs args)
        {
            return FilePathBuilder.Build(args);
        }
    }
}
