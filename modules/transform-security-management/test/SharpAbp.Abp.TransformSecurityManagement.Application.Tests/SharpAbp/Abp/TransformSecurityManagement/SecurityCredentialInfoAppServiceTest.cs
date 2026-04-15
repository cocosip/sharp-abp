using SharpAbp.Abp.CryptoVault;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Xunit;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    public class SecurityCredentialInfoAppServiceTest : TransformSecurityManagementApplicationTestBase
    {
        private readonly ISecurityCredentialInfoAppService _securityCredentialInfoAppService;
        private readonly ISecurityCredentialInfoRepository _securityCredentialInfoRepository;
        private readonly IRSACredsAppService _rsaCredsAppService;

        public SecurityCredentialInfoAppServiceTest()
        {
            _securityCredentialInfoAppService = GetRequiredService<ISecurityCredentialInfoAppService>();
            _securityCredentialInfoRepository = GetRequiredService<ISecurityCredentialInfoRepository>();
            _rsaCredsAppService = GetRequiredService<IRSACredsAppService>();
        }

        [Fact]
        public async Task GetPagedListAsync_Should_Filter_And_Sort_Credentials()
        {
            var alphaId = Guid.NewGuid();
            var betaId = Guid.NewGuid();
            var gammaId = Guid.NewGuid();

            await WithUnitOfWorkAsync(async () =>
            {
                await _securityCredentialInfoRepository.InsertAsync(new SecurityCredentialInfo(
                    alphaId,
                    "alpha",
                    Guid.NewGuid(),
                    "RSA",
                    "Login",
                    new DateTime(2026, 4, 20, 0, 0, 0, DateTimeKind.Utc),
                    "alpha credential"));

                await _securityCredentialInfoRepository.InsertAsync(new SecurityCredentialInfo(
                    betaId,
                    "beta",
                    Guid.NewGuid(),
                    "SM2",
                    "Login",
                    new DateTime(2026, 4, 21, 0, 0, 0, DateTimeKind.Utc),
                    "beta credential"));

                await _securityCredentialInfoRepository.InsertAsync(new SecurityCredentialInfo(
                    gammaId,
                    "gamma",
                    Guid.NewGuid(),
                    "RSA",
                    "Payment",
                    new DateTime(2026, 4, 22, 0, 0, 0, DateTimeKind.Utc),
                    "gamma credential"));
            });

            var pagedResult = await _securityCredentialInfoAppService.GetPagedListAsync(new SecurityCredentialInfoPagedRequestDto
            {
                SkipCount = 0,
                MaxResultCount = 10,
                Sorting = "Identifier asc",
                KeyType = "RSA"
            });

            Assert.Equal(2, pagedResult.TotalCount);
            Assert.Collection(pagedResult.Items,
                item => Assert.Equal("alpha", item.Identifier),
                item => Assert.Equal("gamma", item.Identifier));

            var alpha = await _securityCredentialInfoAppService.GetAsync(alphaId);
            var gamma = await _securityCredentialInfoAppService.FindByIdentifierAsync("gamma");

            Assert.Equal(alphaId, alpha.Id);
            Assert.Equal("RSA", alpha.KeyType);
            Assert.Equal(gammaId, gamma.Id);
            Assert.Equal("Payment", gamma.BizType);
        }

        [Fact]
        public async Task CreateAsync_And_DeleteAsync_Should_Persist_And_Remove_Credential()
        {
            await _rsaCredsAppService.GenerateAsync(new GenerateRSACredsDto
            {
                Count = 1,
                Size = 2048
            });

            await _securityCredentialInfoAppService.CreateAsync(new CreateSecurityCredentialInfoDto
            {
                BizType = "Login"
            });

            var created = await WithUnitOfWorkAsync(async () =>
            {
                var items = await _securityCredentialInfoRepository.GetListAsync(
                    sorting: "CreationTime desc",
                    bizType: "Login");

                return Assert.Single(items);
            });

            Assert.NotEqual(Guid.Empty, created.Id);
            Assert.False(string.IsNullOrWhiteSpace(created.Identifier));
            Assert.Equal("Login", created.BizType);
            Assert.Equal("RSA", created.KeyType);

            var found = await _securityCredentialInfoAppService.FindByIdentifierAsync(created.Identifier);
            Assert.NotNull(found);
            Assert.Equal(created.Id, found.Id);

            await _securityCredentialInfoAppService.DeleteAsync(created.Id);

            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            {
                return _securityCredentialInfoAppService.GetAsync(created.Id);
            });
        }
    }
}
