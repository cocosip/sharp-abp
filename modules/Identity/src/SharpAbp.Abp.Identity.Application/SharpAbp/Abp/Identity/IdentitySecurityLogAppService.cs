﻿using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace SharpAbp.Abp.Identity
{
    [Authorize(IdentityPermissions.IdentitySecurityLogs.Default)]
    public class IdentitySecurityLogAppService : IdentityAppServiceBase, IIdentitySecurityLogAppService
    {
        protected IIdentitySecurityLogRepository SecurityLogRepository { get; }
        public IdentitySecurityLogAppService(IIdentitySecurityLogRepository securityLogRepository)
        {
            SecurityLogRepository = securityLogRepository;
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.IdentitySecurityLogs.Default)]
        public virtual async Task<IdentitySecurityLogDto> GetAsync(Guid id)
        {
            var identitySecurityLog = await SecurityLogRepository.GetAsync(id);
            return ObjectMapper.Map<IdentitySecurityLog, IdentitySecurityLogDto>(identitySecurityLog);
        }

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.IdentitySecurityLogs.Default)]
        public virtual async Task<PagedResultDto<IdentitySecurityLogDto>> GetPagedListAsync(IdentitySecurityLogPagedRequestDto input)
        {
            var count = await SecurityLogRepository.GetCountAsync(
                input.StartTime,
                input.EndTime,
                input.ApplicationName,
                input.Identity,
                input.Action,
                input.UserId,
                input.UserName,
                input.ClientId,
                input.CorrelationId);

            var identitySecurityLogs = await SecurityLogRepository.GetListAsync(
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount,
                input.StartTime,
                input.EndTime,
                input.ApplicationName,
                input.Identity,
                input.Action,
                input.UserId,
                input.UserName,
                input.ClientId,
                input.CorrelationId);

            return new PagedResultDto<IdentitySecurityLogDto>(
              count,
              ObjectMapper.Map<List<IdentitySecurityLog>, List<IdentitySecurityLogDto>>(identitySecurityLogs)
              );
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(IdentityPermissions.IdentitySecurityLogs.Default)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await SecurityLogRepository.DeleteAsync(id);
        }
    }
}
