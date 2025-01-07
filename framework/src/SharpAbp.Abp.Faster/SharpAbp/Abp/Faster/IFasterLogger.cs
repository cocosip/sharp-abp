using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpAbp.Abp.Faster
{

    public interface IFasterLogger : IDisposable
    {

    }

    public interface IFasterLogger<T> : IFasterLogger where T : class
    {
        /// <summary>
        /// 是否已经初始化
        /// </summary>
        bool Initialized { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        void Initialize();

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<long> WriteAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<LogEntryList<T>> ReadAsync(int count = 1, CancellationToken cancellationToken = default);

        /// <summary>
        /// 提交进度
        /// </summary>
        /// <param name="entryPosition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task CommitAsync(LogEntryPosition entryPosition, CancellationToken cancellationToken = default);

    }
}
