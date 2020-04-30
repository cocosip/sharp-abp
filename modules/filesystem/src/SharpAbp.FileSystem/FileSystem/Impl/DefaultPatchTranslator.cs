using DotCommon.Reflecting;
using DotCommon.Utility;
using SharpAbp.FileSystem.FastDFS;
using SharpAbp.FileSystem.S3;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpAbp.FileSystem.Impl
{
    /// <summary>Patch转换器
    /// </summary>
    public class DefaultPatchTranslator : IPatchTranslator
    {

        /// <summary>根据Patch信息生成PatchInfo
        /// </summary>
        public PatchInfo PatchToInfo(Patch patch)
        {
            var patchInfo = new PatchInfo()
            {
                StoreType = StoreType.S3,
                Code = patch.Code,
                Name = patch.Name,
                State = patch.State
            };
            if (patch.StoreType == (int) StoreType.S3)
            {
                var s3Patch = (S3Patch) patch;
                var bucketDict = DictionaryEmit.ObjectToDictionary<BucketInfo>(s3Patch.Bucket);
                patchInfo.Expands.Add(bucketDict.ToList());
            }
            else if (patch.StoreType == (int) StoreType.FastDFS)
            {
                var fastDFSPatch = (FastDFSPatch) patch;
                foreach (var tracker in fastDFSPatch.Trackers)
                {
                    var trackerDict = DictionaryEmit.ObjectToDictionary<Tracker>(tracker);
                    patchInfo.Expands.Add(trackerDict.ToList());
                }

            }
            else
            {
                throw new ArgumentException("不是有效的Patch类型,无法转换成PatchInfo");
            }

            for (int i = 0; i < patchInfo.Expands.Count; i++)
            {
                for (int j = 0; j < patchInfo.Expands[i].Count; j++)
                {
                    var kv = patchInfo.Expands[i][j];
                    var value = kv.Value.TrimEnd('\r').TrimEnd('\n');
                    patchInfo.Expands[i][j] = new KeyValuePair<string, string>(kv.Key, value);
                }
            }

            return patchInfo;
        }

        /// <summary>根据PatchInfo转换成Patch
        /// </summary>
        public Patch InfoToPatch(PatchInfo info)
        {
            if (info.StoreType == StoreType.S3)
            {
                var s3Patch = new S3Patch()
                {
                StoreType = (int) StoreType.S3,
                Name = info.Name,
                Code = info.Code,
                State = info.State
                };
                if (info.Expands.Any())
                {
                    var bucketDict = info.Expands[0].ToDictionary(x => x.Key, x => x.Value);
                    s3Patch.Bucket = DictionaryEmit.DictionaryToObject<BucketInfo>(bucketDict);
                }
                return s3Patch;
            }
            else if (info.StoreType == StoreType.FastDFS)
            {
                var fastDFSPatch = new FastDFSPatch()
                {
                StoreType = (int) StoreType.FastDFS,
                Name = info.Name,
                Code = info.Code,
                State = info.State
                };
                foreach (var item in info.Expands)
                {
                    var trackerDict = item.ToDictionary(x => x.Key, x => x.Value);
                    var tracker = DictionaryEmit.DictionaryToObject<Tracker>(trackerDict);
                    fastDFSPatch.Trackers.Add(tracker);
                }

                return fastDFSPatch;
            }
            else
            {
                throw new ArgumentException("不是有效的Patch类型,无法转换成Patch");
            }

        }
    }
}
