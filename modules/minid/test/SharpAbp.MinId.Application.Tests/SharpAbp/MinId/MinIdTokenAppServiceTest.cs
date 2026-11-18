using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.MinId
{
    public class MinIdTokenAppServiceTest : MinIdApplicationTestBase
    {
        private readonly IMinIdTokenAppService _tokenAppService;
        private readonly IMinIdTokenValidator _tokenValidator;

        public MinIdTokenAppServiceTest()
        {
            _tokenAppService = GetRequiredService<IMinIdTokenAppService>();
            _tokenValidator = GetRequiredService<IMinIdTokenValidator>();
        }

        [Fact]
        public virtual async Task Update_Should_Invalidate_Old_Token_Cache()
        {
            var created = await _tokenAppService.CreateAsync(new CreateMinIdTokenDto
            {
                BizType = "default",
                Token = "oldtoken",
                Remark = "cache regression test"
            });

            Assert.True(await _tokenValidator.ValidateAsync("default", "oldtoken"));

            await _tokenAppService.UpdateAsync(created.Id, new UpdateMinIdTokenDto
            {
                BizType = "default",
                Token = "newtoken",
                Remark = "cache regression test"
            });

            Assert.False(await _tokenValidator.ValidateAsync("default", "oldtoken"));
            Assert.True(await _tokenValidator.ValidateAsync("default", "newtoken"));
        }
    }
}
