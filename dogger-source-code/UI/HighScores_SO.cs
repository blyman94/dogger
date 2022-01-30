using UnityEngine;

namespace Dogger.UI
{
	/// <summary>
	/// Stores all high score information for current and future sessions.
	/// </summary>
	[CreateAssetMenu(fileName = "new HighScores", menuName = "HighScores")]
	public class HighScores_SO : ScriptableObject
	{
		public string FirstPlaceName;
		public int FirstPlaceScore;

		public string SecondPlaceName;
		public int SecondPlaceScore;

		public string ThirdPlaceName;
		public int ThirdPlaceScore;

		/// <summary>
		/// Key string for saving via the JsonUtility and PlayerPrefs.
		/// </summary>
		public string key;

		private void OnEnable()
		{
			// Load state from previous session.
			JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), this);
		}

		private void OnDisable()
		{
			// Save session preferences via JsonUtiltiy and PlayerPrefs.
			if (key == "")
			{
				key = name;
			}

			string jsonData = JsonUtility.ToJson(this, true);
			PlayerPrefs.SetString(key, jsonData);
			PlayerPrefs.Save();
		}

		/// <summary>
		/// Checks the score of the player against the scoreboard and returns
		/// the incoming score's place.
		/// </summary>
		/// <param name="scoreToCheck">The incoming score to compare to
		/// existing high scores.</param>
		/// <returns>Returns placement of the incoming score, or 0 if 
		/// the score did not place.</returns>
		public int CheckScore(int scoreToCheck)
		{
			if (scoreToCheck > FirstPlaceScore)
			{
				return 1;
			}
			if (scoreToCheck <= FirstPlaceScore && scoreToCheck > SecondPlaceScore)
			{
				return 2;
			}
			if (scoreToCheck <= SecondPlaceScore && scoreToCheck > ThirdPlaceScore)
			{
				return 3;
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Updates the stored high scores based on the new score and the place
		/// its in.
		/// </summary>
		/// <param name="name">Name of the score holder.</param>
		/// <param name="scoreToAdd">Score of the score holder.</param>
		/// <param name="place">Placement of the incoming high score.</param>
		public void UpdateScore(string name, int scoreToAdd, int place)
		{
			if (place == 1)
			{
				ThirdPlaceName = SecondPlaceName;
				ThirdPlaceScore = SecondPlaceScore;

				SecondPlaceName = FirstPlaceName;
				SecondPlaceScore = FirstPlaceScore;

				FirstPlaceName = name;
				FirstPlaceScore = scoreToAdd;
			}
			else if (place == 2)
			{
				ThirdPlaceName = SecondPlaceName;
				ThirdPlaceScore = SecondPlaceScore;

				SecondPlaceName = name;
				SecondPlaceScore = scoreToAdd;
			}
			else if (place == 3)
			{
				ThirdPlaceName = name;
				ThirdPlaceScore = scoreToAdd;
			}
		}
	}
}