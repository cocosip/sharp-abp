﻿using JetBrains.Annotations;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    public static class FileProviderSelectorExtensions
    {
        public static IFileProvider Get<TContainer>(
            [NotNull] this IFileProviderSelector selector)
        {
            Check.NotNull(selector, nameof(selector));

            return selector.Get(FileContainerNameAttribute.GetContainerName<TContainer>());
        }
    }
}