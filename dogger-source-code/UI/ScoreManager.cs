using Dogger.Core;
using TMPro;
using UnityEngine;

namespace Dogger.UI
{
    /// <summary>
    /// Collection of data to populate the scoreboard on the High Scores menu.
    /// </summary>
    [System.Serializable]
    public struct HighScoreData
    {
        public TextMeshProUGUI FirstPlaceName;
        public TextMeshProUGUI FirstPlaceScore;
        public HighScores_SO HighScores;

        public TMP_InputField NameInputField;
        public TextMeshProUGUI PlacementText;
        public TextMeshProUGUI SecondPlaceName;
        public TextMeshProUGUI SecondPlaceScore;
        public TextMeshProUGUI ThirdPlaceName;
        public TextMeshProUGUI ThirdPlaceScore;
    }

    /// <summary>
    /// Struct to store references to score summary text located on the Game 
    /// Over screen.
    /// </summary>
    [System.Serializable]
    public struct ScoreSummaryText
    {
        public TextMeshProUGUI CoinsCollectedText;
        public TextMeshProUGUI FinalScoreText;
        public TextMeshProUGUI ScoreText;
    }

    /// <summary>
    /// Class to manage all behavior relating to high scores and end-of-game
    /// player score.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        public HighScoreData HighScoreData;
        public HUDManager hudManager;
        public ScoreSummaryText ScoreSummaryText;
        public SessionPreferences_SO sessionPrefs;

        /// <summary>
        /// Tracks whether the score needs to be updated following a new scene
        /// load.
        /// </summary>
        [HideInInspector]
        public bool updateHighScore = false;

        /// <summary>
        /// Stores placement of the final player score in the leader board.
        /// </summary>
        private int place = 0;

        /// <summary>
        /// Calculate final score of the player run.
        /// </summary>
        /// <returns>Returns the final score of the player run.</returns>
        public int CalculateEndScore()
        {
            int playerScore = Mathf.RoundToInt(hudManager.PlayerScore);
            int playerCoinBonus =
                Mathf.RoundToInt(hudManager.PlayerCoins * 10);
            return (playerScore + playerCoinBonus);
        }

        /// <summary>
        /// Compares the player end of game score to those on the high score
        /// board and determines if the player has placed.
        /// </summary>
        public void DeterminePlacing()
        {
            if (HighScoreData.HighScores != null)
            {
                place = 
                    HighScoreData.HighScores.CheckScore(CalculateEndScore());

                if (place != 0)
                {
                    updateHighScore = true;
                    switch (place)
                    {
                        case 1:
                            HighScoreData.PlacementText.text = 
                                "You Placed First!";
                            break;
                        case 2:
                            HighScoreData.PlacementText.text = 
                                "You Placed Second!";
                            break;
                        case 3:
                            HighScoreData.PlacementText.text = 
                                "You Placed Third!";
                            break;
                    }
                    HighScoreData.NameInputField.gameObject.SetActive(true);
                }
                else
                {
                    updateHighScore = false;

                    HighScoreData.PlacementText.text = "You Did Not Place";
                    HighScoreData.NameInputField.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Updates the high score board located in the High Scores menu with
        /// first through third place names and scores.
        /// </summary>
        public void UpdateHighScoreBoard()
        {
            HighScoreData.FirstPlaceName.text = HighScoreData.HighScores.FirstPlaceName;
            HighScoreData.FirstPlaceScore.text = "" + HighScoreData.HighScores.FirstPlaceScore;

            HighScoreData.SecondPlaceName.text = HighScoreData.HighScores.SecondPlaceName;
            HighScoreData.SecondPlaceScore.text = "" + HighScoreData.HighScores.SecondPlaceScore;

            HighScoreData.ThirdPlaceName.text = HighScoreData.HighScores.ThirdPlaceName;
            HighScoreData.ThirdPlaceScore.text = "" + HighScoreData.HighScores.ThirdPlaceScore;
        }

        /// <summary>
        /// Updates the scriptable object from which high scores are read.
        /// </summary>
        public void UpdateScoreSO()
        {
            if (updateHighScore)
            {
                string highScoreName = HighScoreData.NameInputField.text + " + " + sessionPrefs.DogName;
                if (highScoreName == "")
                {
                    highScoreName = "LIZ + " + sessionPrefs.DogName;
                }
                HighScoreData.HighScores.UpdateScore(highScoreName, CalculateEndScore(), place);
            }
            updateHighScore = false;
            place = 0;
        }

        /// <summary>
        /// Updates the score summary shown at the end of each player run.
        /// </summary>
        public void UpdateScoreSummary()
        {
            if (ScoreSummaryText.ScoreText != null)
            {
                ScoreSummaryText.ScoreText.text = "Score (Distance): " + Mathf.RoundToInt(hudManager.PlayerScore);
            }
            if (ScoreSummaryText.CoinsCollectedText != null)
            {
                ScoreSummaryText.CoinsCollectedText.text = "Coins Collected: " + hudManager.PlayerCoins + " (x10)";
            }
            if (ScoreSummaryText.FinalScoreText != null)
            {
                ScoreSummaryText.FinalScoreText.text = "Final Score: " + CalculateEndScore();
            }
        }
    }
}