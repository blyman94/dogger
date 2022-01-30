using Dogger.Control;
using Dogger.Core;
using Dogger.Dog;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace Dogger.UI
{
    /// <summary>
    /// A class to manage all UI in Dogger. Delegates work to many
    /// sub-managers.
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        public TextMeshProUGUI DogNameText;
        public HUDManager hudManager;
        public Image SceneFaderImage;

        public ScoreManager scoreManager;
        public ScreenManager screenManager;
        public SessionPreferences_SO sessionPrefs;
        public TutorialManager tutorialManager;
        public Toggle tutorialToggle;

        public delegate void NextDog();
        public static NextDog nextDog;
        public delegate void PrevDog();
        public static PrevDog prevDog;

        private void OnEnable()
        {
            PlayerInput.pauseGame += TogglePauseMenu;

            GameManager.gameOver += OnGameOver;
            SceneManager.sceneLoaded += OnSceneLoad;
            SceneManager.sceneLoaded += FadeInScene;

            DogSelector.dogNameChanged += ChangeDogName;
        }

        private void OnDisable()
        {
            PlayerInput.pauseGame -= TogglePauseMenu;

            GameManager.gameOver -= OnGameOver;
            SceneManager.sceneLoaded -= OnSceneLoad;
            SceneManager.sceneLoaded -= FadeInScene;

            DogSelector.dogNameChanged -= ChangeDogName;
        }

        /// <summary>
        /// Buffer function to choose between loading the tutorial or loading
        /// the main scene.
        /// </summary>
        public void ChooseGameScene()
        {
            if (sessionPrefs.Tutorial)
            {
                ToTutorialScene();
            }
            else
            {
                ToMainScene();
            }
        }

        /// <summary>
        /// Triggers scene fade.
        /// </summary>
        /// <param name="scene">(unused)</param>
        /// <param name="loadSceneMode">(unused)</param>
        public void FadeInScene(Scene scene, LoadSceneMode loadSceneMode)
        {
            StartCoroutine(SceneFadeInRoutine());
        }

        /// <summary>
        /// Slowly fades scene in from black.
        /// </summary>
        public IEnumerator SceneFadeInRoutine()
        {
            float startAlpha = 1.0f;
            float endAlpha = 0.0f;
            float elapsedTime = 0.0f;

            Color currentColor = SceneFaderImage.color;

            yield return new WaitForSeconds(1.0f);

            while (elapsedTime <= 2.0f)
            {
                currentColor.a = Mathf.Lerp(startAlpha, endAlpha,
                    elapsedTime / 2.0f);
                SceneFaderImage.color = currentColor;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// Sets game difficulty in session preferences to difficulty selected
        /// by the player.
        /// </summary>
        /// <param name="difficulty">Difficulty selected by player.</param>
        public void SetDifficulty(int difficulty)
        {
            if (difficulty >= 0 && difficulty <= 2)
            {
                sessionPrefs.Difficulty = difficulty;

                switch (sessionPrefs.Difficulty)
                {
                    case 0:
                        hudManager.CurrentSpeed = 1.0f;
                        break;
                    case 1:
                        hudManager.CurrentSpeed = 3.0f;
                        break;
                    case 2:
                        hudManager.CurrentSpeed = 6.0f;
                        break;
                }
            }
            else
            {
                Debug.LogError("[UIManager] Unrecognized difficulty passed to SetDifficulty(int difficulty)");
            }
        }

        /// <summary>
        /// Switch to the next dog avatar in dog selection.
        /// </summary>
        public void SwitchToNextDog()
        {
            nextDog?.Invoke();
        }

        /// <summary>
        /// Switch to the previous dog avatar in dog selection.
        /// </summary>
        public void SwitchToPrevDog()
        {
            prevDog?.Invoke();
        }

        /// <summary>
        /// Pauses the game based on player input.
        /// </summary>
        public void TogglePauseMenu()
        {
            if (screenManager != null)
            {
                if (!screenManager.GameScreens.GameOverScreen.activeInHierarchy)
                {
                    screenManager.GameScreens.PauseScreen.SetActive(!screenManager.GameScreens.PauseScreen.activeInHierarchy);
                }
            }
        }

        /// <summary>
        /// Changes state of tutorial in the session preferences object.
        /// </summary>
        public void ToggleTutorial()
        {
            sessionPrefs.Tutorial = !sessionPrefs.Tutorial;
        }

        /// <summary>
        /// Starts transition to the main scene.
        /// </summary>
        public void ToMainScene()
        {
            StartCoroutine(TransitionScenesRoutine("Main"));
        }

        /// <summary>
        /// Starts transition to the pregame scene.
        /// </summary>
        public void ToPregameScene()
        {
            scoreManager.UpdateScoreSO();
            StartCoroutine(TransitionScenesRoutine("Pregame"));
        }

        /// <summary>
        /// Starts transition to the tutorial scene.
        /// </summary>
        public void ToTutorialScene()
        {
            StartCoroutine(TransitionScenesRoutine("Tutorial"));
        }


        /// <summary>
        /// Routine to transition scenes.
        /// </summary>
        /// <param name="toScene">Scene to transition to.</param>
        public IEnumerator TransitionScenesRoutine(string toScene)
        {
            Time.timeScale = 1;
            float startAlpha = 0.0f;
            float endAlpha = 1.0f;
            float elapsedTime = 0.0f;

            Color currentColor = SceneFaderImage.color;

            while (elapsedTime <= 2.0f)
            {
                currentColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / 2.0f);
                SceneFaderImage.color = currentColor;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (toScene == "Pregame")
            {
                GameManager.LoadPregameScene();
            }
            if (toScene == "Tutorial")
            {
                GameManager.LoadTutorialScene();
            }
            else if (toScene == "Main")
            {
                GameManager.LoadMainScene();
            }
        }

        private void ChangeDogName(string newName)
        {
            DogNameText.text = newName;
        }
        
        /// <summary>
        /// Responds to game over event.
        /// </summary>
        private void OnGameOver()
        {
            if (scoreManager != null)
            {
                scoreManager.UpdateScoreSummary();
                scoreManager.DeterminePlacing();
            }
            if (screenManager != null)
            {
                screenManager.GameScreens.GameOverScreen.SetActive(true);
            }
        }

        /// <summary>
        /// Responds to load scene event.
        /// </summary>
        /// <param name="scene">Name of the currently loaded scene.</param>
        /// <param name="loadSceneMode">(unused)</param>
        private void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name == "Pregame")
            {
                if (scoreManager != null)
                {
                    scoreManager.UpdateHighScoreBoard();
                }
                if (screenManager != null)
                {
                    screenManager.GameScreens.CloseAllScreens();
                    screenManager.GameScreens.ResetPregameScreens();
                    screenManager.GameScreens.TitleScreen.SetActive(true);
                }
                tutorialToggle.isOn = sessionPrefs.Tutorial;
            }
            if (scene.name == "Tutorial" || scene.name == "Main")
            {
                if (hudManager != null)
                {
                    hudManager.PlayerHealth = 3;
                    hudManager.PlayerCoins = 0;
                    hudManager.OnPlayerCoinChange(0);
                    hudManager.PlayerScore = 0;
                }

                if (screenManager != null)
                {
                    screenManager.GameScreens.CloseAllScreens();
                    screenManager.GameScreens.HudScreen.SetActive(true);
                    if (scene.name == "Tutorial")
                    {
                        tutorialManager.StartTutorial();
                    }
                }
            }
        }
    }
}