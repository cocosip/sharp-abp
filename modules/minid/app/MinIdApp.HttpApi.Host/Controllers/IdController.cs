//using Microsoft.AspNetCore.Mvc;
//using SharpAbp.MinId;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Volo.Abp;

//namespace MinIdApp.Controllers
//{
//    [RemoteService(Name = MinIdRemoteServiceConsts.RemoteServiceName)]
//    [Area("minid")]
//    [Route("api/id")]
//    public class IdController : MinIdController
//    {
//        private readonly IMinIdGeneratorFactory _minIdGeneratorFactory;
//        private readonly ISegmentIdService _segmentIdService;
//        private readonly ITokenValidator _tokenValidator;
//        public IdController(
//            IMinIdGeneratorFactory minIdGeneratorFactory,
//            ISegmentIdService segmentIdService,
//            ITokenValidator tokenValidator)
//        {
//            _minIdGeneratorFactory = minIdGeneratorFactory;
//            _segmentIdService = segmentIdService;
//            _tokenValidator = tokenValidator;
//        }


//        [HttpGet]
//        [Route("nextId")]
//        public async Task<long> NextIdAsync(string bizType, string token)
//        {
//            await ValidateTokenAsync(bizType, token);
//            try
//            {
//                var minIdGenerator = await _minIdGeneratorFactory.GetAsync(bizType);
//                return await minIdGenerator.NextIdAsync();
//            }
//            catch (Exception ex)
//            {
//                throw new UserFriendlyException($"Get id failed ,{ex.Message}");
//            }
//        }

//        [HttpGet]
//        [Route("batch-nextId")]
//        public virtual async Task<List<long>> BatchNextIdAsync(string bizType, string token, int batchSize)
//        {
//            await ValidateTokenAsync(bizType, token);
//            try
//            {
//                batchSize = batchSize < 1 ? 1 : batchSize;
//                var minIdGenerator = await _minIdGeneratorFactory.GetAsync(bizType);
//                return await minIdGenerator.NextIdAsync(batchSize);
//            }
//            catch (Exception ex)
//            {
//                throw new UserFriendlyException($"Get id failed ,{ex.Message}");
//            }
//        }

//        [HttpGet]
//        [Route("next-segmentId")]
//        public virtual async Task<SegmentId> NextSegmentIdAsync(string bizType, string token)
//        {
//            await ValidateTokenAsync(bizType, token);
//            try
//            {
//                var segmentId = await _segmentIdService.GetNextSegmentIdAsync(bizType);
//                return segmentId;
//            }
//            catch (Exception ex)
//            {
//                throw new UserFriendlyException($"Get segmentId failed , {ex.Message}");
//            }
//        }


//        private async Task ValidateTokenAsync(string bizType, string token)
//        {
//            if (!await _tokenValidator.ValidateAsync(bizType, token))
//            {
//                throw new UserFriendlyException("Invalid token.");
//            }
//        }
//    }
//}
