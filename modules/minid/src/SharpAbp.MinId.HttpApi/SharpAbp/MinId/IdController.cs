using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;

namespace SharpAbp.MinId
{
    [RemoteService(Name = MinIdRemoteServiceConsts.RemoteServiceName)]
    [Area("minid")]
    [Route("api/id")]
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
        public async Task<long> SimpleNextIdAsync(string bizType, string token)
        {
            await _minIdTokenValidator.ValidateTokenAsync(bizType, token);
            return await _minIdGeneratorAppService.NextIdAsync(bizType);
        }


        [HttpGet]
        [Route("nextId")]
        public async Task<MinIdResponse<long>> NextIdAsync(string bizType, string token)
        {
            try
            {
                await _minIdTokenValidator.ValidateTokenAsync(bizType, token);
                var id = await _minIdGeneratorAppService.NextIdAsync(bizType);
                return MinIdResponse.Successful(id);
            }
            catch (Exception ex)
            {
                return MinIdResponse.Failed<long>(ex.Message);
            }
        }


        [HttpGet]
        [Route("simple/batch-nextId")]
        public async Task<List<long>> SimpleBatchNextIdAsync(string bizType, string token, int batchSize)
        {
            await _minIdTokenValidator.ValidateTokenAsync(bizType, token);
            return await _minIdGeneratorAppService.NextIdAsync(bizType, batchSize);
        }

        [HttpGet]
        [Route("batch-nextId")]
        public async Task<MinIdResponse<List<long>>> BatchNextIdAsync(string bizType, string token, int batchSize)
        {
            try
            {
                await _minIdTokenValidator.ValidateTokenAsync(bizType, token);
                var ids = await _minIdGeneratorAppService.NextIdAsync(bizType, batchSize);
                return MinIdResponse.Successful(ids);
            }
            catch (Exception ex)
            {
                return MinIdResponse.Failed<List<long>>(ex.Message);
            }
        }

        /// <summary>
        /// Get next segmentId
        /// </summary>
        /// <param name="bizType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("next-segmentId")]
        public async Task<SegmentIdDto> GetNextSegmentIdAsync(string bizType)
        {
            return await _segmentIdAppService.GetNextSegmentIdAsync(bizType);
        }
    }
}
