namespace Randomizer
{
    /// <summary>
    /// Represents a range of values - used for possible ranges of randomly generated values
    /// </summary>
    public class Range
	{
		public int MinValue { get; set; }
		public int MaxValue { get; set; }

		/// <summary>
		/// Constructor - has safety checks for whether the min and max values are correct
		/// </summary>
		/// <param name="minValue">The minimum value in the range</param>
		/// <param name="maxValue">The maximum value in the range</param>
		public Range(int minValue, int maxValue)
		{
			if (minValue < maxValue)
			{
				MinValue = minValue;
				MaxValue = maxValue;
			}

			else
			{
				MinValue = maxValue;
				MaxValue = minValue;
			}
		}

        /// <summary>
        /// Gets a random value between the min and max value, inclusive
        /// </summary>
        /// <param name="rng">The RNG object to use</param>
        /// <returns />
        public int GetRandomValue(RNG rng)
			=> rng.NextIntWithinRange(this);

		/// <summary>
		/// Checks if the given value is within the range (inclusive)
		/// </summary>
		/// <param name="value">The value to check</param>
		/// <returns />
		public bool Contains(int value)
			=> value >= MinValue && value <= MaxValue;
    }
}
