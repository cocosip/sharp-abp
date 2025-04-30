using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring
{
    public interface IFileProviderValuesValidator
    {
        string Provider { get; }

        IAbpValidationResult Validate(List<NameValue> values);
    }
}
