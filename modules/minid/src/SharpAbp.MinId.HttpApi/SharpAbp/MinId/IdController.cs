using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Authorization;

namespace SharpAbp.MinId
{
    [RemoteService(Name = MinIdRemoteServiceConsts.RemoteServiceName)]
    [ApiController]
    [Area("minid")]
    [Route("api/minid/id")]
    public class IdController : MinIdController, IRemoteService
    {
        private readonly IMinIdTokenValidator _minIdTokenValidator;
        private readonly IMinIdGeneratorAppService _minIdGeneratorAppService;
        private readonly ISegmentIdAppService _segmentIdAppService;

        public IdController(
            IMinIdTokenValidator minIdTokenValidator,
            IMinIdGeneratorAppService minIdGeneratorAppService,
            ISegmentIdAppService segmentIdAppService)
        {
            _minIdTokenValidator = minIdTokenValidator;
            _minIdGeneratorAppService = minIdGeneratorAppService;
            _segmentIdAppService = segmentIdAppService;
        }


        [HttpGet]
        [Route("simple/nextId")]
        public async Task<long> SimpleNextIdAsync(string bizType, [FromHeader(Name = "Authorization"), DisableAuditing] string token)
        {
            await _minIdTokenValidator.ValidateTokenAsync(bizType, GetBearerToken(token), HttpContext?.RequestAborted ?? default);
            return await _minIdGeneratorAppService.NextIdAsync(bizType);
        }


        [HttpGet]
        [Route("nextId")]
        public async Task<MinIdResponse<long>> NextIdAsync(string bizType, [FromHeader(Name = "Authorization"), DisableAuditing] string token)
        {
            try
            {
                await _minIdTokenValidator.ValidateTokenAsync(bizType, GetBearerToken(token), HttpContext?.RequestAborted ?? default);
                var id = await _minIdGeneratorAppService.NextIdAsync(bizType);
                return MinIdResponse.Successful(id);
            }
            catch (OperationCanceledException) { throw; }
            catch (AbpAuthorizationException)
            {
                return MinIdResponse.Failed<long>("Invalid token.");
            }
            catch (Exception ex) { return UnexpectedFailure<long>(ex); }
        }


        [HttpGet]
        [Route("simple/batch-nextId")]
        public async Task<List<long>> SimpleBatchNextIdAsync(string bizType, [FromHeader(Name = "Authorization"), DisableAuditing] string token,
            [Range(1, MinIdUtil.MaxBatchSize)] int batchSize)
        {
            MinIdUtil.CheckBatchSize(batchSize);
            await _minIdTokenValidator.ValidateTokenAsync(bizType, GetBearerToken(token), HttpContext?.RequestAborted ?? default);
            return await _minIdGeneratorAppService.NextIdAsync(bizType, batchSize);
        }

        [HttpGet]
        [Route("batch-nextId")]
        public async Task<MinIdResponse<List<long>>> BatchNextIdAsync(string bizType, [FromHeader(Name = "Authorization"), DisableAuditing] string token,
            [Range(1, MinIdUtil.MaxBatchSize)] int batchSize)
        {
            try
            {
                MinIdUtil.CheckBatchSize(batchSize);
                await _minIdTokenValidator.ValidateTokenAsync(bizType, GetBearerToken(token), HttpContext?.RequestAborted ?? default);
                var ids = await _minIdGeneratorAppService.NextIdAsync(bizType, batchSize);
                return MinIdResponse.Successful(ids);
            }
            catch (OperationCanceledException) { throw; }
            catch (AbpAuthorizationException)
            {
                return MinIdResponse.Failed<List<long>>("Invalid token.");
            }
            catch (ArgumentOutOfRangeException ex) when (ex.ParamName == nameof(batchSize))
            {
                return MinIdResponse.Failed<List<long>>($"Batch size must be between 1 and {MinIdUtil.MaxBatchSize}.");
            }
            catch (Exception ex) { return UnexpectedFailure<List<long>>(ex); }
        }

        /// <summary>
        /// Get next segmentId
        /// </summary>
        /// <param name="bizType"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("next-segmentId")]
        public async Task<SegmentIdDto> GetNextSegmentIdAsync(string bizType, [FromHeader(Name = "Authorization"), DisableAuditing] string token)
        {
            await _minIdTokenValidator.ValidateTokenAsync(bizType, GetBearerToken(token), HttpContext?.RequestAborted ?? default);
            return await _segmentIdAppService.GetNextSegmentIdAsync(bizType);
        }
    }
}
