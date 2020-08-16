using System;
using System.Collections.Generic;
using System.Text;

namespace SharpAbp.Abp.FileStoring
{
    public interface IFileContainer<TContainer> : IFileContainer where TContainer : class
    {

    }

    public interface IFileContainer
    {

    }
}
