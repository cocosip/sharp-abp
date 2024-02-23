using Volo.Abp.Application.Dtos;

namespace SharpAbp.Abp.CryptoVault
{
    public class SM2CredsPagedRequestDto : PagedAndSortedResultRequestDto
    {
        public string Identifier { get; set; }
        public int? SourceType { get; set; }
        public string Curve { get; set; }
    }
}
