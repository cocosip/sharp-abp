using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class SecurityCredentialInfoToSecurityCredentialInfoDtoMapper : MapperBase<SecurityCredentialInfo, SecurityCredentialInfoDto>
    {
        public override partial SecurityCredentialInfoDto Map(SecurityCredentialInfo source);
        public override partial void Map(SecurityCredentialInfo source, SecurityCredentialInfoDto destination);
    }
}
