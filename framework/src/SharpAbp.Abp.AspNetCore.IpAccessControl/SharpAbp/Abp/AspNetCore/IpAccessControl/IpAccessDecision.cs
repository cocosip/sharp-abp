using System.Net;

namespace SharpAbp.Abp.AspNetCore.IpAccessControl
{
    public enum IpAccessDecisionReason
    {
        AllowedByDefault,
        AllowedAddress,
        DeniedAddress,
        NotAllowlisted,
        ClientAddressUnavailable
    }

    public readonly record struct IpAccessDecision
    {
        public bool IsAllowed { get; init; }
        public IPAddress? ClientIpAddress { get; init; }
        public IpAccessDecisionReason Reason { get; init; }
    }
}
