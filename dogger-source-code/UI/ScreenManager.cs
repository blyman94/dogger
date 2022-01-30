using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Dogger.UI
{
    /// <summary>
    /// Struct to hold all buttons in the UI for easy enabling/disabling during
    /// transitions.
    /// </summary>
    [System.Serializable]
    public struct Buttons
    {
        public Button[] AllButtons;

        public void DisableAllButtons()
        {
            foreach (Button button in AllButtons)
            {
                button.interactable = false;
            }
        }
        public void EnableAllButtons()
        {
            foreach (Button button in AllButtons)
            {
                button.interactable = true;
            }
        }
    }

    /// <summary>
    /// Struct to hold all game screens that can be transitioned between in the
    /// pregame menu, as well as the screens showed during the main scene.
    /// </summary>
    [System.Serializable]
    public struct GameScreens
    {
        public GameObject DifficultyScreen;
        public GameObject DogSelectionScreen;
        public GameObject EmptyScreen;
        public GameObject GameOverScreen;
        public GameObject HighScoreScreen;
        public GameObject HudScreen;
        public GameObject PauseScreen;
        public GameObject TitleScreen;
        public void CloseAllScreens()
        {
            TitleScreen.SetActive(false);
            DifficultyScreen.SetActive(false);
            DogSelectionScreen.SetActive(false);
            HighScoreScreen.SetActive(false);
            PauseScreen.SetActive(false);
            HudScreen.SetActive(false);
            GameOverScreen.SetActive(false);
            EmptyScreen.SetActive(false);
        }

        public void ResetPregameScreens()
        {
            TitleScreen.GetComponent<RectTransform>().localPosition = 
                new Vector3(0, 0, 0);
            DifficultyScreen.GetComponent<RectTransform>().localPosition = 
                new Vector3(800, 0, 0);
            DogSelectionScreen.GetComponent<RectTransform>().localPosition = 
                new Vector3(800, 0, 0);
            EmptyScreen.GetComponent<RectTransform>().localPosition = 
                new Vector3(800, 0, 0);
            HighScoreScreen.GetComponent<RectTransform>().localPosition = 
                new Vector3(-800, 0, 0);
        }
    }

    /// <summary>
    /// Manages screen transitions.
    /// </summary>
    public class ScreenManager : MonoBehaviour
    {
        public Buttons Buttons;
        public GameScreens GameScreens = new GameScreens();

        /// <summary>
        /// Determines whether the tutorial will be launched after dog 
        /// selection.
        /// </summary>
        public Toggle TutorialToggle;

        public UIManager uiManager;

        public delegate void SwitchedFromDogSelect();
        public static SwitchedFromDogSelect switchedFromDogSelect;
        public delegate void SwitchedToDogSelect();
        public static SwitchedToDogSelect switchedToDogSelect;

        // Screen Transition Methods
        public void DifficultyToDogSelect()
        {
            StartCoroutine(TransitionScreensRoutine(GameScreens.DifficultyScreen, 
                GameScreens.DogSelectionScreen, 1.0f));
        }

        public void DifficultyToTitle()
        {
            StartCoroutine(TransitionScreensRoutine(GameScreens.DifficultyScreen,
                GameScreens.TitleScreen, 1.0f));
        }

        public void DogSelectToDifficulty()
        {
            StartCoroutine(TransitionScreensRoutine(GameScreens.DogSelectionScreen, 
                GameScreens.DifficultyScreen, 1.0f));
        }

        public void DogSelectToMainScene()
        {
            StartCoroutine(TransitionScreensRoutine(GameScreens.DogSelectionScreen, 
                GameScreens.EmptyScreen, 1.0f));
        }

        public void HighScoresToTitle()
        {
            StartCoroutine(TransitionScreensRoutine(GameScreens.HighScoreScreen,
                GameScreens.TitleScreen, 1.0f));
        }

        public void TitleToDifficulty()
        {
            StartCoroutine(TransitionScreensRoutine(GameScreens.TitleScreen, 
                GameScreens.DifficultyScreen, 1.0f));
        }

        public void TitleToHighScores()
        {
            StartCoroutine(TransitionScreensRoutine(GameScreens.TitleScreen,
                GameScreens.HighScoreScreen, 1.0f));
        }

        /// <summary>
        /// Transitions between the two given scenes over the transition time.
        /// </summary>
        /// <param name="firstScreen">Screen to transition from.</param>
        /// <param name="secondScreen">Screen to transition to.</param>
        /// <param name="transitionTime">Transition Duration</param>
        public IEnumerator TransitionScreensRoutine(GameObject firstScreen,
            GameObject secondScreen, float transitionTime)
        {
            Buttons.DisableAllButtons();
            TutorialToggle.interactable = false;
            if (firstScreen == GameScreens.DogSelectionScreen && 
                secondScreen == GameScreens.DifficultyScreen)
            {
                switchedFromDogSelect?.Invoke();
            }

            firstScreen.SetActive(true);
            secondScreen.SetActive(true);
            Vector3 centerScreen = 
                firstScreen.GetComponent<RectTransform>().localPosition;
            Vector3 secondScreenStartPos = 
                secondScreen.GetComponent<RectTransform>().localPosition;
            float firstScreenMoveDir = 
                centerScreen.x < secondScreenStartPos.x ? -1 : 1;
            Vector3 firstScreenEndPos = 
                centerScreen + new Vector3(800.0f * firstScreenMoveDir, 0, 0);
            float elapsedTime = 0.0f;
            while (elapsedTime <= transitionTime)
            {
                firstScreen.GetComponent<RectTransform>().localPosition = 
                    Vector3.Lerp(centerScreen, firstScreenEndPos,
                    elapsedTime / transitionTime);
                secondScreen.GetComponent<RectTransform>().localPosition = 
                    Vector3.Lerp(secondScreenStartPos, centerScreen,
                    elapsedTime / transitionTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            firstScreen.SetActive(false);

            if (firstScreen == GameScreens.DifficultyScreen && 
                secondScreen == GameScreens.DogSelectionScreen)
            {
                switchedToDogSelect?.Invoke();
            }

            if (firstScreen == GameScreens.DogSelectionScreen && 
                secondScreen == GameScreens.EmptyScreen)
            {
                uiManager.ChooseGameScene();
            }
            Buttons.EnableAllButtons();
            TutorialToggle.interactable = true;
        }
    }
}