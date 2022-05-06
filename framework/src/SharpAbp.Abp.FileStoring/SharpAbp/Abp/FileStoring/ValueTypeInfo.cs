using System;

namespace SharpAbp.Abp.FileStoring
{
    public class ValueTypeInfo
    {
        public Type Type { get; set; }

        /// <summary>
        /// Example
        /// </summary>
        public string Eg { get; set; }

        public ValueTypeInfo()
        {

        }

        public ValueTypeInfo(Type type, string eg)
        {
            Type = type;
            Eg = eg;
        }
    }
}