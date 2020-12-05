using System;

namespace SharpAbp.Abp.FileStoring
{
    public class FileProviderValue
    {
        public Type Type { get; set; }

        public string Note { get; set; }

        public FileProviderValue()
        {

        }

        public FileProviderValue(Type type, string note)
        {
            Type = type;
            Note = note;
        }
    }
}
