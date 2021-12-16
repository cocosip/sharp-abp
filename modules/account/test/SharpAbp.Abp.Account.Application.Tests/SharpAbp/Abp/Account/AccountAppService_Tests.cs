using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Xunit;

namespace SharpAbp.Abp.Account
{
    public class AccountAppService_Tests : AccountApplicationTestBase
    {
        private readonly IAccountAppService _accountAppService;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly ILookupNormalizer _lookupNormalizer;
        private readonly IdentityUserManager _userManager;
        private readonly IOptions<IdentityOptions> _identityOptions;

        public AccountAppService_Tests()
        {
            _accountAppService = GetRequiredService<IAccountAppService>();
            _identityUserRepository = GetRequiredService<IIdentityUserRepository>();
            _lookupNormalizer = GetRequiredService<ILookupNormalizer>();
            _userManager = GetRequiredService<IdentityUserManager>();
            _identityOptions = GetRequiredService<IOptions<IdentityOptions>>();
        }

        [Fact]
        public async Task RegisterAsync()
        {
            await _identityOptions.SetAsync();

            var registerDto = new RegisterDto
            {
                UserName = "bob.lee",
                EmailAddress = "bob.lee@abp.io",
                Password = "P@ssW0rd",
                AppName = "MVC"
            };

            await _accountAppService.RegisterAsync(registerDto);

            var user = await _identityUserRepository.FindByNormalizedUserNameAsync(
                _lookupNormalizer.NormalizeName("bob.lee"));

            user.ShouldNotBeNull();
            user.UserName.ShouldBe("bob.lee");
            user.Email.ShouldBe("bob.lee@abp.io");

            (await _userManager.CheckPasswordAsync(user, "P@ssW0rd")).ShouldBeTrue();
        }
    }
}
