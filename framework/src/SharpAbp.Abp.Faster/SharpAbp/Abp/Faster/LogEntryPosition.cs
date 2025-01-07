namespace SharpAbp.Abp.Faster
{
    public class LogEntryPosition
    {

        public Position Min { get; set; }
        public Position Max { get; set; }

        public LogEntryPosition()
        {

        }

        public LogEntryPosition(Position min, Position max)
        {
            Min = min;
            Max = max;
        }
    }
}
