using System;

namespace SharpAbp.Abp.FileStoring
{
    public interface IFileContainerConfigurationTypeConverter
    {
        Type TargetType { get; set; }

        object ConvertType(string value);
    }
}
