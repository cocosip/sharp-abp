using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.Aliyun
{
    public class DefaultAliyunFileNameCalculator : IAliyunFileNameCalculator, ITransientDependency
    {
        protected IFilePathBuilder FilePathBuilder { get; }

        public DefaultAliyunFileNameCalculator(IFilePathBuilder filePathBuilder)
        {
            FilePathBuilder = filePathBuilder;
        }

        public virtual string Calculate(FileProviderArgs args)
        {
            return FilePathBuilder.Build(args);
        }
    }
}
