using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.Account.Web.ProfileManagement;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Account.Web.Pages.Account
{
    public class ManageModel : AccountPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }
        
        public ProfileManagementPageCreationContext ProfileManagementPageCreationContext { get; private set; }

        protected ProfileManagementPageOptions Options { get; }

        public ManageModel(IOptions<ProfileManagementPageOptions> options)
        {
            Options = options.Value;
        }

        public virtual async Task<IActionResult> OnGetAsync()
        {
            ProfileManagementPageCreationContext = new ProfileManagementPageCreationContext(ServiceProvider);

            foreach (var contributor in Options.Contributors)
            {
                await contributor.ConfigureAsync(ProfileManagementPageCreationContext);
            }

            return Page();
        }

        public virtual Task<IActionResult> OnPostAsync()
        {
            return Task.FromResult<IActionResult>(Page());
        }
    }
}
