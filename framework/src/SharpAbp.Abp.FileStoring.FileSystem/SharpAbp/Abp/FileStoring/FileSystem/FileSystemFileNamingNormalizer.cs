using System.IO;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring.FileSystem
{
    public class FileSystemFileNamingNormalizer : IFileNamingNormalizer, ITransientDependency
    {
        private readonly IOSPlatformProvider _iosPlatformProvider;

        public FileSystemFileNamingNormalizer(IOSPlatformProvider iosPlatformProvider)
        {
            _iosPlatformProvider = iosPlatformProvider;
        }

        public virtual string NormalizeContainerName(string containerName)
        {
            return Normalize(containerName);
        }

        public virtual string NormalizeFileName(string fileName)
        {
            return Normalize(fileName);
        }

        protected virtual string Normalize(string name)
        {
            if (Path.IsPathRooted(name) || name.Contains(":") || ContainsTraversalSegment(name))
            {
                throw new AbpException($"The value '{name}' must be a relative path without traversal segments.");
            }

            var os = _iosPlatformProvider.GetCurrentOSPlatform();
            if (os == OSPlatform.Windows)
            {
                // A filename cannot contain any of the following characters: \ / : * ? " < > |
                // In order to support the directory included in the blob name, remove / and \
                name = Regex.Replace(name, "[:\\*\\?\"<>\\|]", string.Empty);
            }

            return name;
        }

        protected virtual bool ContainsTraversalSegment(string name)
        {
            foreach (var segment in name.Split(['/', '\\'], System.StringSplitOptions.None))
            {
                if (segment == "." || segment == "..")
                {
                    return true;
                }
            }

            return false;
        }
    }
}
