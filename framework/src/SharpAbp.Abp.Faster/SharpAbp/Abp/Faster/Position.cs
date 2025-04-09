using System;

namespace SharpAbp.Abp.Faster
{
    public class Position : IComparable<Position>
    {
        public long Address { get; set; }
        public long Length { get; set; }
        public long NextAddress { get; set; }
        public Position()
        {

        }
        public Position(long address, long length, long nextAddress)
        {
            Address = address;
            Length = length;
            NextAddress = nextAddress;
        }

        public bool IsMatch(long nextAddress)
        {
            return nextAddress == Address || Math.Abs(nextAddress - Address) < 10;
        }

        public int CompareTo(Position other)
        {
            if (other == null)
            {
                return 1;
            }
            int addressComparison = Address.CompareTo(other.Address);
            if (addressComparison != 0)
            {
                return addressComparison;
            }
            return Length.CompareTo(other.Length);
        }
    }

    public class RetryPosition
    {
        public Position? Position { get; set; }
        public int RetryCount { get; set; }

        public RetryPosition()
        {

        }

        public RetryPosition(Position position, int retryCount)
        {
            Position = position;
            RetryCount = retryCount;
        }


        public bool IsMax(int max)
        {
            return RetryCount >= max && max > 0;
        }



    }
}
