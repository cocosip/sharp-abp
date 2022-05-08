using System.Threading;

namespace SharpAbp.MinId
{
    public class SegmentId
    {
        private object _sync = new();

        private long _currentId;

        public long CurrentId
        {
            get { return _currentId; }
            set { _currentId = value; }
        }

        public long MaxId { get; set; }
        public int Delta { get; set; }
        public int Remainder { get; set; }
        public long LoadingId { get; set; }

        private bool _isSetup = false;

        public bool IsUseful()
        {
            return CurrentId <= MaxId;
        }

        public void Setup()
        {
            if (_isSetup)
            {
                return;
            }

            lock (_sync)
            {
                if (_isSetup)
                {
                    return;
                }

                if (CurrentId % Delta == Remainder)
                {
                    _isSetup = true;
                    return;
                }

                var id = CurrentId;

                for (int i = 0; i <= Delta; i++)
                {
                    id = Interlocked.Increment(ref _currentId);
                    if (id % Delta == Remainder)
                    {
                        // 避免浪费 减掉系统自己占用的一个id
                        Interlocked.Add(ref _currentId, (0 - Delta));
                        _isSetup = true;
                        return;
                    }
                }
            }

        }


        public (int, long) NextId()
        {
            Setup();

            var id = Interlocked.Add(ref _currentId, Delta);

            if (id > MaxId)
            {
                return new(ResultCodeConsts.Over, id);
            }
            if (id >= LoadingId)
            {
                return new(ResultCodeConsts.Loading, id);
            }
            return new(ResultCodeConsts.Normal, id);
        }

    }
}
