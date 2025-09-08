using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Volo.Abp.Validation;

namespace SharpAbp.Abp.Validation
{
    /// <summary>
    /// Provides validation helper methods for common validation scenarios.
    /// </summary>
    public static class ValidateHelper
    {
        /// <summary>
        /// Validates that a string value is not null or whitespace.
        /// </summary>
        /// <param name="result">The validation result to add errors to</param>
        /// <param name="provider">The provider name for error context</param>
        /// <param name="name">The field name for error context</param>
        /// <param name="value">The value to validate</param>
        public static void NotNullOrWhiteSpace(IAbpValidationResult result, string provider, string name, string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                result.Errors.Add(new ValidationResult($"[{provider}-{name}] value should not be null or whiteSpace."));
            }
        }

        /// <summary>
        /// Validates that a string value represents a valid boolean.
        /// </summary>
        /// <param name="result">The validation result to add errors to</param>
        /// <param name="provider">The provider name for error context</param>
        /// <param name="name">The field name for error context</param>
        /// <param name="value">The value to validate</param>
        public static void ShouldBool(IAbpValidationResult result, string provider, string name, string value)
        {
            if (!IsBoolean(value))
            {
                result.Errors.Add(new ValidationResult($"[{provider}-{name}] value is not a bool type value : '{value}' ."));
            }
        }

        /// <summary>
        /// Validates that a string value represents a valid integer.
        /// </summary>
        /// <param name="result">The validation result to add errors to</param>
        /// <param name="provider">The provider name for error context</param>
        /// <param name="name">The field name for error context</param>
        /// <param name="value">The value to validate</param>
        public static void ShouldInt(IAbpValidationResult result, string provider, string name, string value)
        {
            if (!IsInteger(value))
            {
                result.Errors.Add(new ValidationResult($"[{provider}-{name}] value is not a int type value : '{value}' ."));
            }
        }

        /// <summary>
        /// Determines whether the specified string value represents a valid boolean.
        /// </summary>
        /// <param name="value">The string value to check</param>
        /// <returns>true if the value is a valid boolean representation; otherwise, false</returns>
        public static bool IsBoolean(string value)
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

        /// <summary>
        /// Determines whether the specified string value represents a valid integer.
        /// </summary>
        /// <param name="value">The string value to check</param>
        /// <returns>true if the value is a valid integer representation; otherwise, false</returns>
        public static bool IsInteger(string value)
        {
            if (!value.IsNullOrWhiteSpace())
            {
                return Regex.IsMatch(value, @"^(\-|\+)?[0-9]*$");
            }
            return false;
        }
    }
}