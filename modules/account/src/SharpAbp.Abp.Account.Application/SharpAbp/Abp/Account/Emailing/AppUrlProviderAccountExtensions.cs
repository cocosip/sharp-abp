using System.Threading.Tasks;
using Volo.Abp.UI.Navigation.Urls;

namespace SharpAbp.Abp.Account.Emailing
{
    public static class  AppUrlProviderAccountExtensions
    {
        public static Task<string> GetResetPasswordUrlAsync(this IAppUrlProvider appUrlProvider, string appName)
        {
            return appUrlProvider.GetUrlAsync(appName, AccountUrlNames.PasswordReset);
        }
    }
}
