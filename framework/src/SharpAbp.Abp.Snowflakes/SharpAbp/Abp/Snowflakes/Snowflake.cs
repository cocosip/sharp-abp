using System;

namespace SharpAbp.Abp.Snowflakes
{
    /// <summary>
    /// 雪花算法分布式id生成
    /// Twitter_Snowflake
    /// SnowFlake的结构如下(每部分用-分开):
    /// 0 - 0000000000 0000000000 0000000000 0000000000 0 - 00000 - 00000 - 000000000000
    /// 1位标识，由于long基本类型在Java中是带符号的，最高位是符号位，正数是0，负数是1，所以id一般是正数，最高位是0
    /// 41位时间截(毫秒级)，注意，41位时间截不是存储当前时间的时间截，而是存储时间截的差值（当前时间截 - 开始时间截)
    /// 得到的值），这里的的开始时间截，一般是我们的id生成器开始使用的时间，由我们程序来指定的（如下下面程序IdWorker类的startTime属性）。41位的时间截，可以使用69年，年T = (1L 《 41) / (1000L * 60 * 60 * 24 * 365) = 69
    /// 10位的数据机器位，可以部署在1024个节点，包括5位datacenterId和5位workerId
    /// 12位序列，毫秒内的计数，12位的计数顺序号支持每个节点每毫秒(同一机器，同一时间截)产生4096个ID序号
    /// 加起来刚好64位，为一个Long型
    /// SnowFlake的优点是，整体上按照时间自增排序，并且整个分布式系统内不会产生ID碰撞(由数据中心ID和机器ID作区分)，并且效率较高，经测试,SnowFlake每秒能够产生26万ID左右。
    /// </summary>
    public class Snowflake
    {

        //开始时间截(2015-01-01)
        private readonly long _twepoch = 1420041600000L;

        //机器id所占的位数
        private readonly int _workerIdBits = 5;

        //数据标识id所占的位数
        private readonly int _datacenterIdBits = 5;

        //序列在id中占的位数(1ms内的并发数)
        private readonly int _sequenceBits = 12;

        //机器ID向左移12位
        private readonly int _workerIdShift = 12;

        //数据标识id向左移17位(12+5)
        private readonly int _datacenterIdShift = 17;

        //时间截向左移22位(5+5+12)
        private readonly int _timestampLeftShift = 22;

        // 生成序列的掩码，这里为4095 (0b111111111111=0xfff=4095)
        private readonly long _sequenceMask = 4095;

        // 工作机器ID(0~31)
        private readonly long _workerId;

        //数据中心ID(0~31)
        private readonly long _datacenterId;

        //毫秒内序列(0~4095)
        private long sequence = 0L;

        //上次生成ID的时间截
        private long lastTimestamp = -1L;

        private readonly object _sync = new();

        private static readonly Lazy<Snowflake> _instance = new Lazy<Snowflake>(() =>
        {
            return new Snowflake(0L, 0L);
        });

        public static Snowflake GetInstance { get { return _instance.Value; } }

        public Snowflake(long workerId = 0L, long datacenterId = 0L)
            : this(1420041600000L, 5, 5, 12, workerId, datacenterId)
        {

        }


        public Snowflake(
            long twepoch = 1420041600000L,
            int workerIdBits = 5,
            int datacenterIdBits = 5,
            int sequenceBits = 12,
            long workerId = 0L,
            long datacenterId = 0L)
        {
            _twepoch = twepoch;
            _workerIdBits = workerIdBits;
            _datacenterIdBits = datacenterIdBits;
            _sequenceBits = sequenceBits;
            _workerIdShift = _sequenceBits;

            _datacenterIdShift = _sequenceBits + _workerIdBits;
            _timestampLeftShift = _sequenceBits + _workerIdBits + _datacenterIdBits;
            _sequenceMask = -1L ^ (-1L << _sequenceBits);

            _workerId = workerId;
            _datacenterId = datacenterId;

            var maxWorkerId = -1L ^ (-1L << _workerIdBits);
            var maxDatacenterId = -1L ^ (-1L << _datacenterIdBits);

            if (workerId > maxWorkerId || workerId < 0)
            {
                throw new ArgumentException(string.Format("worker Id can't be greater than %d or less than 0", maxWorkerId));
            }
            if (datacenterId > maxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException(string.Format("datacenter Id can't be greater than %d or less than 0", maxDatacenterId));
            }
        }



        /// <summary>
        /// 获得下一个ID (该方法是线程安全的)
        /// </summary>
        public long NextId()
        {
            lock (_sync)
            {
                long timestamp = TimeGen();

                //如果当前时间小于上一次ID生成的时间戳，说明系统时钟回退过这个时候应当抛出异常
                if (timestamp < lastTimestamp)
                {
                    throw new InvalidTimeZoneException(
                            string.Format("Clock moved backwards.  Refusing to generate id for %d milliseconds", lastTimestamp - timestamp));
                }

                //如果是同一时间生成的，则进行毫秒内序列
                if (lastTimestamp == timestamp)
                {
                    sequence = (sequence + 1) & _sequenceMask;
                    //毫秒内序列溢出
                    if (sequence == 0)
                    {
                        //阻塞到下一个毫秒,获得新的时间戳
                        timestamp = TilNextMillis(lastTimestamp);
                    }
                }
                //时间戳改变，毫秒内序列重置
                else
                {
                    sequence = 0L;
                }

                //上次生成ID的时间截
                lastTimestamp = timestamp;

                //移位并通过或运算拼到一起组成64位的ID
                return ((timestamp - _twepoch) << _timestampLeftShift) //
                        | (_datacenterId << _datacenterIdShift) //
                        | (_workerId << _workerIdShift) //
                        | sequence;
            }
        }




        /// <summary>
        /// 阻塞到下一个毫秒，直到获得新的时间戳
        /// </summary>
        private long TilNextMillis(long lastTimestamp)
        {
            long timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        /// <summary>
        /// 返回以毫秒为单位的当前时间
        /// </summary>
        protected long TimeGen()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }


    }
}
