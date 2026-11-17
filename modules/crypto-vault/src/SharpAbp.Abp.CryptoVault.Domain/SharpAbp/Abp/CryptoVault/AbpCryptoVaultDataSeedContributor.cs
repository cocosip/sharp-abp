using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.CryptoVault
{
    public class AbpCryptoVaultDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly AbpCryptoVaultOptions _options;
        private readonly IKeyService _keyService;
        private readonly IRSACredsRepository _rsaCredsRepository;
        private readonly ISM2CredsRepository _sm2CredsRepository;
        public AbpCryptoVaultDataSeedContributor(
            IOptions<AbpCryptoVaultOptions> options,
            IKeyService keyService,
            IRSACredsRepository rsaCredsRepository,
            ISM2CredsRepository sm2CredsRepository)
        {
            _options = options.Value;
            _keyService = keyService;
            _rsaCredsRepository = rsaCredsRepository;
            _sm2CredsRepository = sm2CredsRepository;
        }


        public async Task SeedAsync(DataSeedContext context)
        {
            if (context.TenantId.HasValue)
            {
                return;
            }

            var rsaCount = await _rsaCredsRepository.GetCountAsync();
            var rsaToGenerate = _options.RSACount - rsaCount;
            if (rsaToGenerate > 0)
            {
                var rsaCredsList = _keyService.GenerateRSACreds(_options.RSAKeySize, rsaToGenerate);
                await _rsaCredsRepository.InsertManyAsync(rsaCredsList);
            }

            var sm2Count = await _sm2CredsRepository.GetCountAsync();
            var sm2ToGenerate = _options.SM2Count - sm2Count;
            if (sm2ToGenerate > 0)
            {
                var sm2CredsList = _keyService.GenerateSM2Creds(_options.SM2Curve, sm2ToGenerate);
                await _sm2CredsRepository.InsertManyAsync(sm2CredsList);
            }
        }
    }
}
