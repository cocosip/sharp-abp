using AutoMapper;

namespace SharpAbp.Abp.CryptoVault
{
    public class CryptoVaultApplicationModuleAutoMapperProfile : Profile
    {
        public CryptoVaultApplicationModuleAutoMapperProfile()
        {
            CreateMap<RSACreds, RSACredsDto>();
            CreateMap<SM2Creds, SM2CredsDto>();
        }
    }
}
