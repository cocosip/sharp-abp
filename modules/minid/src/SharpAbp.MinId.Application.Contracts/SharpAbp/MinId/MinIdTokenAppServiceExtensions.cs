using System.Threading.Tasks;
using Volo.Abp;

namespace SharpAbp.MinId;

public static class MinIdTokenAppServiceExtensions
{
    public static Task<MinIdTokenDto> FindAsync(this IMinIdTokenAppService service, FindMinIdTokenDto input)
    {
        Check.NotNull(input, nameof(input));
        return service.FindByTokenAsync(input.BizType, input.Token);
    }
}
