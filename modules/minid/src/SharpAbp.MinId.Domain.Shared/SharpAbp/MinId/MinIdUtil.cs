using System;
using System.Text.RegularExpressions;

namespace SharpAbp.MinId
{
    public static class MinIdUtil
    {
        public const int MaxBatchSize = 1000;

        public static void CheckBatchSize(int batchSize)
        {
            if (batchSize < 1 || batchSize > MaxBatchSize)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize), $"Batch size must be between 1 and {MaxBatchSize}.");
            }
        }

        public static bool IsToken(string input)
        {
            if (!input.IsNullOrWhiteSpace())
            {
                var pattern = "^[A-Za-z0-9]{3,40}$";

                return Regex.IsMatch(input, pattern);
            }
            return false;
        }


        public static bool IsBizType(string input)
        {
            if (!input.IsNullOrWhiteSpace())
            {
                var pattern = "^[A-Za-z0-9]{3,32}$";

                return Regex.IsMatch(input, pattern);
            }
            return false;
        } 
    }
}
