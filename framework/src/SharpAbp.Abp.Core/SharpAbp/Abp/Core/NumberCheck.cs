using JetBrains.Annotations;
using System;
using System.Diagnostics;

namespace SharpAbp.Abp.Core
{
    [DebuggerStepThrough]
    public static class NumberCheck
    {
        [ContractAnnotation("number: not positive => halt")]
        public static int Positive(
            int number,
            [InvokerParameterName][NotNull] string parameterName)
        {
            if (number <= 0)
            {
                throw new ArgumentException($"{parameterName} is not positive!", parameterName);
            }        
            return number;
        }

        [ContractAnnotation("number: not positive => halt")]
        public static long Positive(
            long number,
            [InvokerParameterName][NotNull] string parameterName)
        {
            if (number <= 0)
            {
                throw new ArgumentException($"{parameterName} is not positive!", parameterName);
            }
            return number;
        }

        [ContractAnnotation("number: negative => halt")]
        public static int Nonnegative(
            int number,
            [InvokerParameterName][NotNull] string parameterName)
        {
            if (number < 0)
            {
                throw new ArgumentException($"{parameterName} is negative!", parameterName);
            }
            return number;
        }

        [ContractAnnotation("number: negative => halt")]
        public static long Nonnegative(
            long number,
            [InvokerParameterName][NotNull] string parameterName)
        {
            if (number < 0)
            {
                throw new ArgumentException($"{parameterName} is negative!", parameterName);
            }
            return number;
        }

    }
}
