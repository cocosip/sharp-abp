using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace SharpAbp.Abp.CryptoVault
{

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class RSACredsToRSACredsDtoMapper : MapperBase<RSACreds, RSACredsDto>
    {
        public override partial RSACredsDto Map(RSACreds source);
        public override partial void Map(RSACreds source, RSACredsDto destination);
    }


    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class SM2CredsToSM2CredsDtoMapper : MapperBase<SM2Creds, SM2CredsDto>
    {
        public override partial SM2CredsDto Map(SM2Creds source);
        public override partial void Map(SM2Creds source, SM2CredsDto destination);
    }

}
