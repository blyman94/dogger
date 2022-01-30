using UnityEngine;

namespace Dogger.Core
{
	/// <summary>
	/// Stores the difficulty, tutorial selection, and dog selection 
	/// information for a player's current session.
	/// </summary>
	[CreateAssetMenu(fileName = "new SessionPrefs", menuName = "SessionPrefs")]
	public class SessionPreferences_SO : ScriptableObject
	{
		/// <summary>
		/// The game difficulty chosen by the player.
		/// </summary>
		public int Difficulty = 0;

		public int DogID = 0;
		public string DogName = "Korra";

		/// <summary>
		/// Represents the player's selection of whether or not to launch the
		/// tutorial.
		/// </summary>
		public bool Tutorial = true;

		private void OnDisable()
		{
			Reset();
		}

		private void Reset()
		{
			Difficulty = 0;
			DogID = 0;
			DogName = "Korra";
			Tutorial = true;
		}
	}
}