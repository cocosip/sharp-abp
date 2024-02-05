namespace SharpAbp.Abp.AspNetCore.Http
{
    public interface IRemoteIpAddressAccessor
    {

        /// <summary>
        /// Get remote ip address
        /// </summary>
        /// <returns></returns>
        string GetRemoteIpAddress();

        /// <summary>
        /// Get remote ip address from X-Forwarded-For
        /// </summary>
        /// <returns></returns>
        string GetXForwardedForRemoteIpAddress();

        /// <summary>
        /// Get remote ip address from X-Real-IP
        /// </summary>
        /// <returns></returns>
        string GetXRealIPRemoteIpAddress();
    }
}