namespace Dogger.Player
{
    /// <summary>
    /// The health pool of the player.
    /// </summary>
    public class Health
    {
        public delegate void HealthChange(int newHealth);
        public static HealthChange healthChanged;

        /// <summary>
        /// Current health property.
        /// </summary>
        public int Current { get; set; }

        /// <summary>
        /// Max health property.
        /// </summary>
        public int Max { get; set; }

        /// <summary>
        /// Constructor for the health class. Sets max and current health to a
        /// specified value.
        /// </summary>
        /// <param name="max">Maximum health of the player.</param>
        public Health(int max)
        {
            Max = max;
            Current = Max;
            healthChanged?.Invoke(Current);
        }

        /// <summary>
        /// Decreases Current health by the given amount.
        /// </summary>
        /// <param name="amount">Amount by which to reduce health.</param>
        public void Decrement(int amount)
        {
            Current -= amount;
            healthChanged?.Invoke(Current);
        }
    }
}

