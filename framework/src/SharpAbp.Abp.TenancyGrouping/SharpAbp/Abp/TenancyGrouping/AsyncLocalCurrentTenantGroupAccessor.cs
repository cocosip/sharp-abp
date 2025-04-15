using System.Threading;

namespace SharpAbp.Abp.TenancyGrouping
{
    public class AsyncLocalCurrentTenantGroupAccessor : ICurrentTenantGroupAccessor
    {
        public static AsyncLocalCurrentTenantGroupAccessor Instance { get; } = new();

        public BasicTenantGroupInfo? Current
        {
            get => _currentScope.Value;
            set => _currentScope.Value = value;
        }

        private readonly AsyncLocal<BasicTenantGroupInfo?> _currentScope;

        private AsyncLocalCurrentTenantGroupAccessor()
        {
            _currentScope = new AsyncLocal<BasicTenantGroupInfo?>();
        }
    }
}
