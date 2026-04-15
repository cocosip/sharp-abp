namespace SharpAbp.Abp.AspNetCore.Http
{
    public interface IRemoteIpAddressAccessor
    {

        /// <summary>
        /// Get remote ip address
        /// </summary>
        /// <returns></returns>
        string GetRemoteIpAddress();
    }
}
