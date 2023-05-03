using UnityEngine;

namespace Watermelon
{
    /// <summary>
    /// RDS Randomizer implementation. This is a static class and should replace the use of .net's Random class.
    /// It is a bit slower than random due to the fact, that it uses RNGCryptoServiceProvider to produce more real random numbers,
    /// but the benefit of this is a far less predictable sequence of numbers.
    /// You may replace the randomizer used by calling the SetRandomizer method with any object derived from Random.
    /// Supply NULL to SetRandomizer to reset it to the default RNGCryptoServiceProvider.
    /// </summary>
    public static class WeightRandom
    {
        /// <summary>
        /// Retrieves the next random value from the random number generator.
        /// The result is always between 0.0 and the given max-value (excluding).
        /// </summary>
        /// <param name="max">The maximum value.</param>
        /// <returns>A random double value</returns>
        public static float GetValue(float max)
        {
            return Random.Range(0, max);
        }

        /// <summary>
        /// Retrieves the next double random value from the random number generator.
        /// The result is always between the given min-value (including) and the given max-value (excluding).
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>A random double value</returns>
        public static float GetValue(float min, float max)
        {
            return Random.Range(min, max);
        }
    }
}