using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.CryptoVault
{
    public class RSACredsPagedRequestDto : PagedAndSortedResultRequestDto
    {
        public string Identifier { get; set; }
        public int? SourceType { get; set; }
        public int? Size { get; set; }
    }
}
