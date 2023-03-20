using System.Collections.Generic;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring
{
    public interface IFileProviderValuesValidator
    {
        string Provider { get; }

        IAbpValidationResult Validate(Dictionary<string, string> keyValuePairs);
    }
}
