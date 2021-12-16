using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Volo.Abp.Account;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.Account
{
    public class AbpAccountApplicationModuleAutoMapperProfile : Profile
    {
        public AbpAccountApplicationModuleAutoMapperProfile()
        {
            CreateMap<IdentityUser, ProfileDto>()
                .ForMember(dest => dest.HasPassword,
                    op => op.MapFrom(src => src.PasswordHash != null))
                .MapExtraProperties();
        }
    }
}