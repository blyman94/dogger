using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using Dogger.Player;
using Dogger.Dog;
using Dogger.Spawning;
using Dogger.Core;

namespace Dogger.UI
{
    /// <summary>
    /// Struct to store all text displayed on the HUD during the tutorial and
    /// main scenes.
    /// </summary>
    [System.Serializable]
    public struct HudText
    {
        public TextMeshProUGUI CoinText;
        public TextMeshProUGUI DebuffedText;
        public TextMeshProUGUI DogAteWasteText;
        public TextMeshProUGUI HealthText;
        public TextMeshProUGUI ScoreText;
    }

    /// <summary>
    /// Manages updates to the player heads up display (HUD) based on events
    /// that occur in game.
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        public HudText HudText;
        public SessionPreferences_SO sessionPrefs;

        /// <summary>
        /// List of strings to choose from when dog eats waste.
        /// </summary>
        private readonly string[] dogAteWasteStrings = new string[6];

        public delegate void PlayerHealthIsZero();
        public static PlayerHealthIsZero playerHealthIsZero;

        public float CurrentSpeed { get; set; }
        public float PlayerCoins { get; set; }
        public float PlayerHealth { get; set; }
        public float PlayerScore { get; set; }

        private void OnEnable()
        {
            Health.healthChanged += OnPlayerHealthChange;
            Bank.coinChange += OnPlayerCoinChange;
            SpawnManager.currentSpeedChanged += OnSpeedChange;

            PlayerCollisionHandler.collidedWithObstacle += EmphasizeHealthMag;
            PlayerCollisionHandler.collidedWithWaste += ShowDebuffedText;
            PlayerCollisionHandler.debuffEnded += HideDebuffedText;

            DogCharacter.pickedUpWaste += EmphasizeHealthYel;
            DogCharacter.pickedUpWaste += ShowDogAteWasteText;

            SceneManager.sceneLoaded += InitializeDogAteWasteStrings;
            SceneManager.sceneLoaded += InitializeHud;
        }

        private void Update()
        {
            UpdatePlayerScore();
        }

        private void OnDisable()
        {
            Health.healthChanged -= OnPlayerHealthChange;
            Bank.coinChange -= OnPlayerCoinChange;
            SpawnManager.currentSpeedChanged -= OnSpeedChange;

            PlayerCollisionHandler.collidedWithObstacle -= EmphasizeHealthMag;
            PlayerCollisionHandler.collidedWithWaste -= ShowDebuffedText;
            PlayerCollisionHandler.debuffEnded -= HideDebuffedText;

            DogCharacter.pickedUpWaste -= EmphasizeHealthYel;
            DogCharacter.pickedUpWaste -= ShowDogAteWasteText;

            SceneManager.sceneLoaded -= InitializeDogAteWasteStrings;
            SceneManager.sceneLoaded -= InitializeHud;
        }

        /// <summary>
        /// Updates HUD when player collects a coin.
        /// </summary>
        /// <param name="newCoins">The new total coins count to display</param>
        public void OnPlayerCoinChange(int newCoins)
        {
            PlayerCoins = newCoins;
            if (HudText.CoinText != null)
            {
                HudText.CoinText.text = "COINS: " + newCoins;
            }
        }

        /// <summary>
        /// Enlarges and recolors the health text display, to make sure the 
        /// player is aware they have lost health from an obstacle hit.
        /// </summary>
        /// <param name="health">Current health of the player (unused)</param>
        private void EmphasizeHealthMag(int health)
        {
            StartCoroutine(EmphasizeHealthRoutine(Color.magenta));
        }

        private IEnumerator EmphasizeHealthRoutine(Color color)
        {
            if (HudText.HealthText != null)
            {
                float origSize = 24.0f;
                HudText.HealthText.fontSize = origSize * 1.5f;
                HudText.HealthText.color = color;
                yield return new WaitForSeconds(1.5f);
                HudText.HealthText.fontSize = origSize;
                HudText.HealthText.color = Color.white;
            }
        }

        /// <summary>
        /// Enlarges and recolors the health text display, to make sure the 
        /// player is aware they have lost health from their dog eating waste.
        /// </summary>
        /// <param name="health">Current health of the player (unused)</param>
        private void EmphasizeHealthYel(int health)
        {
            StartCoroutine(EmphasizeHealthRoutine(Color.yellow));
        }

        private void HideDebuffedText()
        {
            HudText.DebuffedText.gameObject.SetActive(false);
        }

        /// <summary>
        /// Initializes a set of 6 possible strings to appear in the HUD when 
        /// the players dog eats waste. Plugs in the selected dogs name for a 
        /// more persoanlized touch.
        /// </summary>
        /// <param name="scene">Current scene (unused)</param>
        /// <param name="loadSceneMode">Load Scene Mode (unused)</param>
        private void InitializeDogAteWasteStrings(Scene scene, LoadSceneMode loadSceneMode)
        {
            dogAteWasteStrings[0] = $"Gross! {sessionPrefs.DogName}, don't eat that!";
            dogAteWasteStrings[1] = $"Ugh... Come on {sessionPrefs.DogName}!";
            dogAteWasteStrings[2] = $"Why do you do this {sessionPrefs.DogName}!?";
            dogAteWasteStrings[3] = $"Stop it {sessionPrefs.DogName}! That's not food!";
            dogAteWasteStrings[4] = "Uh oh... Should I call the vet?";
            dogAteWasteStrings[5] = "Who is leaving their waste about?";
        }

        private void InitializeHud(Scene scene, LoadSceneMode loadSceneMode)
        {
            HudText.HealthText.fontSize = 24;
            HudText.DebuffedText.gameObject.SetActive(false);
            PlayerScore = 0;
            PlayerCoins = 0;
            PlayerHealth = 3;
        }
        
        /// <summary>
        /// Updates HUD when player loses health.
        /// </summary>
        /// <param name="newHealth">The new health amount to be 
        /// displayed</param>
        private void OnPlayerHealthChange(int newHealth)
        {
            PlayerHealth = newHealth;
            if (HudText.HealthText != null)
            {
                HudText.HealthText.text = "HEALTH: " + newHealth;
            }
            if (newHealth <= 0)
            {
                if (HudText.HealthText != null)
                {
                    HudText.HealthText.text = "HEALTH: 0";
                }
                playerHealthIsZero?.Invoke();
            }
        }

        private void OnSpeedChange(float currentSpeed)
        {
            CurrentSpeed = currentSpeed;
        }

        private void ShowDebuffedText()
        {
            HudText.DebuffedText.gameObject.SetActive(true);
        }

        private void ShowDogAteWasteText(int health)
        {
            if (health > 0)
            {
                StartCoroutine(ShowDogAteWasteTextRoutine());
            }
        }

        /// <summary>
        /// Selects a random string to show when the dog eats waste. Shows the
        /// selected string in the HUD for 1.5 seconds.
        /// </summary>
        private IEnumerator ShowDogAteWasteTextRoutine()
        {
            int randomInt = Random.Range(0, dogAteWasteStrings.Length);
            HudText.DogAteWasteText.text = dogAteWasteStrings[randomInt];
            HudText.DogAteWasteText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            HudText.DogAteWasteText.gameObject.SetActive(false);
        }

        /// <summary>
        /// Updates the player score based on distance traveled.
        /// </summary>
        private void UpdatePlayerScore()
        {
            PlayerScore += CurrentSpeed * Time.deltaTime;
            if (HudText.ScoreText != null)
            {
                HudText.ScoreText.text = "SCORE: " + Mathf.RoundToInt(PlayerScore) + "m";
            }
        }
    }
}