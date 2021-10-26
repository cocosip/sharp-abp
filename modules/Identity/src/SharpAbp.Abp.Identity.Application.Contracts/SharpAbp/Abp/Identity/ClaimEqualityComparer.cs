using System.Collections.Generic;
using System.Security.Claims;

namespace SharpAbp.Abp.Identity
{
    public class ClaimEqualityComparer : IEqualityComparer<Claim>
    {
        public bool Equals(Claim x, Claim y)
        {
            if (x == null)
            {
                return y == null;
            }
            if (y == null)
            {
                return x == null;
            }
            return x.Type == y.Type && x.Value == y.Value;
        }

        public int GetHashCode(Claim obj)
        {
            return obj.Type.GetHashCode() & obj.Value.GetHashCode();
        }
    }
}
