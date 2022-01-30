using Dogger.Control;
using Dogger.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dogger.Core
{
    /// <summary>
    /// Class responsible for game state management (paused, game over, or
    /// neither) and scene transitions.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        public static GameOver gameOver;
        public delegate void GameOver();

        public bool IsGameOver { get; set; }
        public bool IsGamePaused { get; set; }
        public bool Testing { get; set; }

        private void Start()
        {
            if (!Testing)
            {
                LoadPregameScene();
            }
        }

        private void OnEnable()
        {
            PlayerInput.pauseGame += TogglePause;
            HUDManager.playerHealthIsZero += OnPlayerHealthEmpty;
            SceneManager.sceneLoaded += OnSceneLoad;
        }

        private void OnDisable()
        {
            PlayerInput.pauseGame -= TogglePause;
            HUDManager.playerHealthIsZero -= OnPlayerHealthEmpty;
            SceneManager.sceneLoaded -= OnSceneLoad;
        }

        public static void LoadMainScene()
        {
            SceneManager.LoadScene(3);
        }

        public static void LoadPregameScene()
        {
            SceneManager.LoadScene(1);
        }

        public static void LoadTutorialScene()
        {
            SceneManager.LoadScene(2);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        /// <summary>
        /// Responds to player pause input by stopping time and changing
        /// current state.
        /// </summary>
        public void TogglePause()
        {
            if (!IsGameOver)
            {
                if (IsGamePaused)
                {
                    if (Time.timeScale == 0)
                    {
                        Time.timeScale = 1;
                    }
                    IsGamePaused = false;
                }
                else
                {
                    if (Time.timeScale == 1)
                    {
                        Time.timeScale = 0;
                    }
                    IsGamePaused = true;
                }
            }
        }

        /// <summary>
        /// Method called when player loses the game.
        /// </summary>
        private void EndGame()
        {
            if (!IsGameOver)
            {
                Time.timeScale = 0;
                IsGamePaused = true;
                IsGameOver = true;
                gameOver?.Invoke();
            }
        }

        private void OnPlayerHealthEmpty()
        {
            EndGame();
        }

        private void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
        {
            Time.timeScale = 1;
            IsGamePaused = false;
            IsGameOver = false;
        }
    }
}