using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.Identity
{
    public class IdentityClaimTypeDto : ExtensibleEntityDto<Guid>
    {
        public string Name { get; set; }

        public bool Required { get; set; }

        public bool IsStatic { get; set; }

        public string Regex { get; set; }

        public string RegexDescription { get; set; }

        public string Description { get; set; }

        public virtual IdentityClaimValueType ValueType { get; set; }
    }
}
