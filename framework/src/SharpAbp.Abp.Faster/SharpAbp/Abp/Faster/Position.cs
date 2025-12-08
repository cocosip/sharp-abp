using System;

namespace SharpAbp.Abp.Faster
{
    /// <summary>
    /// Represents an address range with a start and end position.
    /// </summary>
    public interface IAddressRange
    {
        /// <summary>
        /// Gets the start address of the range.
        /// </summary>
        long Start { get; }

        /// <summary>
        /// Gets the end address of the range.
        /// </summary>
        long End { get; }
    }

    /// <summary>
    /// Represents a position in the log with address range information.
    /// </summary>
    public class Position : IAddressRange, IComparable<Position>
    {
        /// <summary>
        /// Gets or sets the starting address of this position.
        /// </summary>
        public long Address { get; set; }

        /// <summary>
        /// Gets or sets the next address after this position (end of range).
        /// </summary>
        public long NextAddress { get; set; }

        /// <summary>
        /// Gets the start address (same as Address).
        /// </summary>
        long IAddressRange.Start => Address;

        /// <summary>
        /// Gets the end address (same as NextAddress).
        /// </summary>
        long IAddressRange.End => NextAddress;

        /// <summary>
        /// Gets the length of this position's range.
        /// </summary>
        public long Length => NextAddress - Address;

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> class.
        /// </summary>
        public Position()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> class.
        /// </summary>
        /// <param name="address">The starting address.</param>
        /// <param name="nextAddress">The next address (end of range).</param>
        /// <exception cref="ArgumentException">Thrown when address is negative or nextAddress is not greater than address.</exception>
        public Position(long address, long nextAddress)
        {
            if (address < 0)
            {
                throw new ArgumentException($"Address must be non-negative. Got: {address}", nameof(address));
            }

            if (nextAddress <= address)
            {
                throw new ArgumentException(
                    $"NextAddress must be greater than Address. Address: {address}, NextAddress: {nextAddress}",
                    nameof(nextAddress));
            }

            Address = address;
            NextAddress = nextAddress;
        }

        /// <summary>
        /// Validates this position's address range.
        /// </summary>
        /// <returns>True if the position is valid, otherwise false.</returns>
        public bool IsValid()
        {
            return Address >= 0 && NextAddress > Address;
        }

        /// <summary>
        /// Compares this position to another based on address.
        /// </summary>
        public int CompareTo(Position? other)
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

            return NextAddress.CompareTo(other.NextAddress);
        }

        /// <summary>
        /// Returns a string representation of this position.
        /// </summary>
        public override string ToString()
        {
            return $"[{Address}, {NextAddress})";
        }
    }

}
