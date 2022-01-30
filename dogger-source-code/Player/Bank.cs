namespace Dogger.Player
{
	/// <summary>
	/// The player's coin collection system.
	/// </summary>
	public class Bank
	{
		public int CoinsCollected { get; set; }

		public delegate void CoinChange(int newCoin);
		public static CoinChange coinChange;

		public void AddCoin(int amount)
		{
			CoinsCollected += amount;
			coinChange?.Invoke(CoinsCollected);
		}
	}

}