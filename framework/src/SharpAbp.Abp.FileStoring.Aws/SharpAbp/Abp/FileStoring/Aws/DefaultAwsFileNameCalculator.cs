using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.Aws
{
    public class DefaultAwsFileNameCalculator : IAwsFileNameCalculator, ITransientDependency
    {
        protected IFilePathBuilder FilePathBuilder { get; }

        public DefaultAwsFileNameCalculator(IFilePathBuilder filePathBuilder)
        {
            FilePathBuilder = filePathBuilder;
        }

        public virtual string Calculate(FileProviderArgs args)
        {
            return FilePathBuilder.Build(args);
        }
    }
}
