namespace SharpAbp.Abp.SM
{
    public interface ISm3EncryptionService
    {
        /// <summary>
        /// 使用SM3获取Hash
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <returns></returns>
        byte[] GetHash(byte[] plainText);
    }
}
