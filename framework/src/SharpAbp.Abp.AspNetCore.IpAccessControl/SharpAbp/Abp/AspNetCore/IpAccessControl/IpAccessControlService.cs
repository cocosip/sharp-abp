using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SharpAbp.Abp.AspNetCore.Http;
using System;
using System.Linq;
using System.Net;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.AspNetCore.IpAccessControl
{
    public class IpAccessControlService : IIpAccessControlService, ITransientDependency
    {
        protected AbpIpAccessControlOptions Options { get; }
        protected IHttpRequestInfoAccessor RequestInfoAccessor { get; }
        protected ILogger<IpAccessControlService> Logger { get; }

        public IpAccessControlService(
            IOptions<AbpIpAccessControlOptions> options,
            IHttpRequestInfoAccessor requestInfoAccessor)
            : this(options, requestInfoAccessor, NullLogger<IpAccessControlService>.Instance)
        {
        }

        public IpAccessControlService(
            IOptions<AbpIpAccessControlOptions> options,
            IHttpRequestInfoAccessor requestInfoAccessor,
            ILogger<IpAccessControlService> logger)
        {
            Options = options.Value;
            RequestInfoAccessor = requestInfoAccessor;
            Logger = logger;
        }

        public virtual IpAccessDecision EvaluateCurrentRequest()
        {
            var requestInfo = RequestInfoAccessor.GetRequestInfo();
            var decision = Evaluate(requestInfo);
            if (!decision.IsAllowed)
            {
                Logger.LogWarning("IP access denied. Reason: {Reason}; ResolutionStatus: {ResolutionStatus}; " +
                    "Source: {Source}; ClientIpAddress: {ClientIpAddress}; ConnectionIpAddress: {ConnectionIpAddress}; " +
                    "ConnectionPort: {ConnectionPort}.", decision.Reason, requestInfo.ResolutionStatus,
                    requestInfo.Source, decision.ClientIpAddress, requestInfo.ConnectionIpAddress, requestInfo.ConnectionPort);
            }
            return decision;
        }

        public virtual IpAccessDecision Evaluate(HttpRequestInfo requestInfo)
        {
            if (!Options.IsEnabled)
            {
                return new IpAccessDecision
                {
                    ClientIpAddress = requestInfo.ResolutionStatus == RequestInfoResolutionStatus.Resolved
                        ? requestInfo.ClientIpAddress == null ? null : Normalize(requestInfo.ClientIpAddress)
                        : null,
                    IsAllowed = true,
                    Reason = IpAccessDecisionReason.Disabled
                };
            }

            if (requestInfo.ClientIpAddress == null
                || requestInfo.ResolutionStatus != RequestInfoResolutionStatus.Resolved)
            {
                return new IpAccessDecision
                {
                    ClientIpAddress = requestInfo.ClientIpAddress,
                    IsAllowed = !Options.DenyWhenClientAddressUnavailable,
                    Reason = IpAccessDecisionReason.ClientAddressUnavailable
                };
            }

            var clientIpAddress = Normalize(requestInfo.ClientIpAddress);
            if (Matches(clientIpAddress, Options.DeniedAddresses, Options.DeniedNetworks))
            {
                return new IpAccessDecision
                {
                    ClientIpAddress = clientIpAddress,
                    IsAllowed = false,
                    Reason = IpAccessDecisionReason.DeniedAddress
                };
            }

            if (Options.AllowedAddresses.Count == 0 && Options.AllowedNetworks.Count == 0)
            {
                return new IpAccessDecision
                {
                    ClientIpAddress = clientIpAddress,
                    IsAllowed = true,
                    Reason = IpAccessDecisionReason.AllowedByDefault
                };
            }

            var isAllowed = Matches(clientIpAddress, Options.AllowedAddresses, Options.AllowedNetworks);
            return new IpAccessDecision
            {
                ClientIpAddress = clientIpAddress,
                IsAllowed = isAllowed,
                Reason = isAllowed
                    ? IpAccessDecisionReason.AllowedAddress
                    : IpAccessDecisionReason.NotAllowlisted
            };
        }

        public virtual IpAccessDecision Evaluate(IPAddress? clientIpAddress)
        {
            return Evaluate(new HttpRequestInfo
            {
                ClientIpAddress = clientIpAddress,
                ResolutionStatus = clientIpAddress == null
                    ? RequestInfoResolutionStatus.NoClientAddress
                    : RequestInfoResolutionStatus.Resolved
            });
        }

        protected virtual bool Matches(
            IPAddress address,
            System.Collections.Generic.IEnumerable<IPAddress> addresses,
            System.Collections.Generic.IEnumerable<IPNetwork> networks)
        {
            return addresses.Any(item => Equals(Normalize(item), address))
                || Matches(address, networks);
        }

        protected virtual bool Matches(
            IPAddress address,
            System.Collections.Generic.IEnumerable<IPNetwork> networks)
        {
            return networks.Any(network => network.Contains(address));
        }

        protected virtual IPAddress Normalize(IPAddress address)
        {
            return address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address;
        }
    }
}
