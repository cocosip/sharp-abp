using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace SharpAbp.MinId
{
    public class MinIdDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IGuidGenerator _guidGenerator;
        private readonly IMinIdInfoRepository _minIdInfoRepository;
        private readonly IMinIdTokenRepository _minIdTokenRepository;
        public MinIdDataSeedContributor(
            IGuidGenerator guidGenerator,
            IMinIdInfoRepository minIdInfoRepository,
            IMinIdTokenRepository minIdTokenRepository)
        {
            _guidGenerator = guidGenerator;
            _minIdInfoRepository = minIdInfoRepository;
            _minIdTokenRepository = minIdTokenRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _minIdInfoRepository.GetCountAsync(default) > 0)
            {
                return;
            }

            var minIdInfo = new MinIdInfo(_guidGenerator.Create(), "default", 0, 100, 1, 0);
            await _minIdInfoRepository.InsertAsync(minIdInfo);

            var minIdToken = new MinIdToken(_guidGenerator.Create(), "default", Guid.NewGuid().ToString("N"), "default token");
            await _minIdTokenRepository.InsertAsync(minIdToken);
        }
    }
}
