using System;
using System.Threading;
using Volo.Abp;

namespace SharpAbp.Abp.FileStoring
{
    /// <summary>
    /// AsyncLocal-based implementation of <see cref="IFilePathContextAccessor"/>.
    /// Registered as a singleton; context flows through async call chains correctly.
    /// Follows the same pattern as <c>AsyncLocalCurrentTenantGroupAccessor</c>.
    /// </summary>
    public class AsyncLocalFilePathContextAccessor : IFilePathContextAccessor
    {
        public static AsyncLocalFilePathContextAccessor Instance { get; } = new AsyncLocalFilePathContextAccessor();

        public FilePathContext? Current
        {
            get => _currentScope.Value;
            set => _currentScope.Value = value;
        }

        private readonly AsyncLocal<FilePathContext?> _currentScope;

        private AsyncLocalFilePathContextAccessor()
        {
            _currentScope = new AsyncLocal<FilePathContext?>();
        }

        public IDisposable Change(FilePathContext? context)
        {
            var previous = Current;
            Current = context;
            return new DisposeAction<(IFilePathContextAccessor, FilePathContext?)>(
                static state =>
                {
                    var (accessor, prev) = state;
                    accessor.Current = prev;
                },
                (this, previous));
        }
    }
}
