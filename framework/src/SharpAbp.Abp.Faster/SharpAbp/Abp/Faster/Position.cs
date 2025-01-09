using System;

namespace SharpAbp.Abp.Faster
{
    public class Position : IComparable<Position>
    {
        public long Address { get; set; }
        public long Length { get; set; }

        public Position()
        {

        }
        public Position(long address, long length)
        {
            Address = address;
            Length = length;
        }

        public bool IsNext(long currentAddress)
        {
            return currentAddress + Length <= Address || currentAddress == Address;
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


}
