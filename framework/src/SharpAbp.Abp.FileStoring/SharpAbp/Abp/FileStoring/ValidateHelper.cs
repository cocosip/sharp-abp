using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.FileStoring
{
    public static class ValidateHelper
    {
        public static void NotNullOrWhiteSpace(IAbpValidationResult result, string provider, string name, string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                result.Errors.Add(new ValidationResult($"[{provider}-{name}] value should not be null or whiteSpace."));
            }
        }

        public static void ShouldBool(IAbpValidationResult result, string provider, string name, string value)
        {
            if (!IsBoolean(value))
            {
                result.Errors.Add(new ValidationResult($"[{provider}-{name}] value is not a bool type value : '{value}' ."));
            }
        }

        public static void ShouldInt(IAbpValidationResult result, string provider, string name, string value)
        {
            if (!IsInteger(value))
            {
                result.Errors.Add(new ValidationResult($"[{provider}-{name}] value is not a int type value : '{value}' ."));
            }
        }




        internal static bool IsBoolean(string value)
        {
            if (!value.IsNullOrWhiteSpace())
            {
                if (value.Equals("true", StringComparison.OrdinalIgnoreCase) || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsInteger(string value)
        {

            if (!value.IsNullOrWhiteSpace())
            {
                return Regex.IsMatch(value, @"^(\-|\+)?[0-9]*$");
            }
            return false;
        }

    }
}
