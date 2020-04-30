namespace SharpAbp.FileSystem
{
    /// <summary>Patch转换器
    /// </summary>
    public interface IPatchTranslator
    {
        /// <summary>根据Patch信息生成PatchInfo
        /// </summary>
        PatchInfo PatchToInfo(Patch patch);

        /// <summary>根据PatchInfo转换成Patch
        /// </summary>
        Patch InfoToPatch(PatchInfo info);
    }
}
