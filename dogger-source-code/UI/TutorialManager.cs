using Dogger.Core;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dogger.UI
{
	/// <summary>
	/// Manages the tutorial sequence for the player.
	/// </summary>
	public class TutorialManager : MonoBehaviour
	{
		public GameObject CoinPrefab;
		public GameObject ContinueButton;
		public GameObject FinishButton;
		public GameManager gameManager;
		public GameObject ObstaclePrefab;
		public GameObject PolePrefab;
		public GameObject SkipTutorialButton;
		public GameObject TutorialScreen;
		public TextMeshProUGUI TutorialText;
		public GameObject WastePrefab;
		public Button RestartButton;

		/// <summary>
		/// Tracks if current waiting for player to press button.
		/// </summary>
		private bool waitingForPlayer = false;

		/// <summary>
		/// Responds to button click event. Signals no longer waiting for 
		/// player.
		/// </summary>
		public void Continue()
		{
			waitingForPlayer = false;
		}

		/// <summary>
		/// Responds to button click event. Signals cancellation of the
		/// tutorial and subsequent load of the main scene.
		/// </summary>
		public void SkipTutorial()
		{
			RestartButton.interactable = true;
			gameManager.TogglePause();
			TutorialScreen.SetActive(false);
			StopAllCoroutines();
		}

		/// <summary>
		/// Stops the coroutines when the player exits via the pause menu.
		/// </summary>
		public void ExitTutorial()
		{
			RestartButton.interactable = true;
			TutorialScreen.SetActive(false);
			StopAllCoroutines();
		}

		/// <summary>
		/// Kicks off the tutorial routine.
		/// </summary>
		public void StartTutorial()
		{
			StartCoroutine(TutorialRoutine());
		}

		/// <summary>
		/// Spawns a coin at a specific location once for the tutorial.
		/// </summary>
		private void SpawnCoin()
		{
			GameObject coin = Instantiate(CoinPrefab, new Vector3(-1.0f,
				CoinPrefab.transform.position.y, 17.0f), Quaternion.identity);
			coin.GetComponent<Rigidbody>().AddForce(Vector3.back * 2,
				ForceMode.VelocityChange);
		}

		/// <summary>
		/// Spawns an obstacle at a specific location once for the tutorial.
		/// </summary>
		private void SpawnObstacle()
		{
			GameObject obstacle = Instantiate(ObstaclePrefab, 
				new Vector3(2.0f, -1.0f, 17.0f), Quaternion.identity);
			obstacle.GetComponent<Rigidbody>().AddForce(Vector3.back * 2,
				ForceMode.VelocityChange);
		}

		/// <summary>
		/// Spawns a pole at a specific location once for the tutorial.
		/// </summary>
		private void SpawnPole()
		{
			GameObject pole = Instantiate(PolePrefab, new Vector3(0.5f,
				PolePrefab.transform.position.y, 17.0f), Quaternion.identity);
			pole.GetComponent<Rigidbody>().AddForce(Vector3.back * 2,
				ForceMode.VelocityChange);
			pole.gameObject.transform.tag = "Untagged";
			pole.gameObject.layer = 0;
		}

		/// <summary>
		/// Spawns waste at a specific location once for the tutorial.
		/// </summary>
		private void SpawnWaste()
		{
			GameObject waste = Instantiate(WastePrefab,
				new Vector3(0.0f, -1.0f, 17.0f), Quaternion.identity);
			waste.GetComponent<Rigidbody>().AddForce(Vector3.back * 2,
				ForceMode.VelocityChange);
		}

		/// <summary>
		/// Main routine for the tutorial. Sequentially introduces the player 
		/// to gameplay mechanics in a slow, controlled environment. They
		/// player may skip the tutorial at any time.
		/// </summary>
		private IEnumerator TutorialRoutine()
		{
			RestartButton.interactable = false;
			yield return new WaitForSeconds(5.0f);

			// Section 1: Goal and HUD
			TutorialText.text = " Welcome To Dogger! \n This tutorial will" +
				" help you walk your dog safely";
			TutorialScreen.SetActive(true);
			ContinueButton.SetActive(true);
			SkipTutorialButton.SetActive(true);
			FinishButton.SetActive(false);
			gameManager.TogglePause();
			waitingForPlayer = true;
			while (waitingForPlayer)
			{
				yield return null;
			}
			TutorialText.text = "The goal of the game is to walk your " +
				"dog as far as possible.";
			waitingForPlayer = true;
			while (waitingForPlayer)
			{
				yield return null;
			}
			TutorialText.text = "Your total distance, health, and coins " +
				"are tracked at the top left of the screen. Try to beat " +
				"the high scores!";
			waitingForPlayer = true;
			while (waitingForPlayer)
			{
				yield return null;
			}
			gameManager.TogglePause();
			TutorialScreen.SetActive(false);
			yield return new WaitForSeconds(3.0f);

			// Section 2: Controls
			TutorialText.text = "You will move forward automatically. " +
				"Use the A and D keys to move left and right";
			TutorialScreen.SetActive(true);
			gameManager.TogglePause();
			waitingForPlayer = true;
			while (waitingForPlayer)
			{
				yield return null;
			}
			gameManager.TogglePause();
			TutorialScreen.SetActive(false);
			yield return new WaitForSeconds(4.0f);
			TutorialText.text = "Click and drag to the left and to the " +
				"right to move your dog in that direction. You can't drag" +
				" your dog off the sidewalk, though!";
			TutorialScreen.SetActive(true);
			gameManager.TogglePause();
			waitingForPlayer = true;
			while (waitingForPlayer)
			{
				yield return null;
			}
			gameManager.TogglePause();
			TutorialScreen.SetActive(false);


			// Section 3: Coins
			SpawnCoin();
			yield return new WaitForSeconds(5.0f);
			TutorialText.text = "There's a coin! Move to the coin to" +
				" collect it";
			TutorialScreen.SetActive(true);
			gameManager.TogglePause();
			waitingForPlayer = true;
			while (waitingForPlayer)
			{
				yield return null;
			}
			gameManager.TogglePause();
			TutorialScreen.SetActive(false);


			// Section 4: Obstacles
			SpawnObstacle();
			yield return new WaitForSeconds(6.0f);
			TutorialText.text = "Watch out for that planter! Hitting it " +
				"will reduce your health by 1";
			TutorialScreen.SetActive(true);
			gameManager.TogglePause();
			waitingForPlayer = true;
			while (waitingForPlayer)
			{
				yield return null;
			}
			gameManager.TogglePause();
			TutorialScreen.SetActive(false);


			// Section 4: Waste
			SpawnWaste();
			yield return new WaitForSeconds(5.0f);
			TutorialText.text = "Yuck! Someone forgot to pick up after " +
				"their dog";
			TutorialScreen.SetActive(true);
			gameManager.TogglePause();
			waitingForPlayer = true;
			while (waitingForPlayer)
			{
				yield return null;
			}
			TutorialText.text = "Don't let your dog eat the waste. If " +
				"this happens, it will reduce your health by 1";
			waitingForPlayer = true;
			while (waitingForPlayer)
			{
				yield return null;
			}
			TutorialText.text = "If you step in waste, your movement will" +
				" be reversed for a short time because its gross";
			waitingForPlayer = true;
			while (waitingForPlayer)
			{
				yield return null;
			}
			TutorialText.text = "These rules apply to trash, too!";
			waitingForPlayer = true;
			while (waitingForPlayer)
			{
				yield return null;
			}
			gameManager.TogglePause();
			TutorialScreen.SetActive(false);
			yield return new WaitForSeconds(1.0f);


			// Section 4: Poles
			SpawnPole();
			yield return new WaitForSeconds(5.0f);
			TutorialText.text = "There's a pole. A dog-walking nightmare...";
			TutorialScreen.SetActive(true);
			gameManager.TogglePause();
			waitingForPlayer = true;
			while (waitingForPlayer)
			{
				yield return null;
			}
			TutorialText.text = "Don't let your leash get hung up on " +
				"the pole! If it does, your run will end!";
			waitingForPlayer = true;
			while (waitingForPlayer)
			{
				yield return null;
			}
			TutorialText.text = "To prevent this, make sure you and your " +
				"dog are on the same side of the pole when you pass it";
			waitingForPlayer = true;
			while (waitingForPlayer)
			{
				yield return null;
			}
			TutorialText.text = "If you run into the pole directly, " +
				"your health will be reduced by 1";
			waitingForPlayer = true;
			while (waitingForPlayer)
			{
				yield return null;
			}
			gameManager.TogglePause();
			TutorialScreen.SetActive(false);
			yield return new WaitForSeconds(6.0f);

			// Section 5: Wrap Up
			TutorialText.text = "Great! That's all you need to know";
			TutorialScreen.SetActive(true);
			gameManager.TogglePause();
			waitingForPlayer = true;
			while (waitingForPlayer)
			{
				yield return null;
			}
			TutorialText.text = "Remember to check the High Scores from " +
				"the main menu, to see how you are doing!";
			waitingForPlayer = true;
			while (waitingForPlayer)
			{
				yield return null;
			}
			ContinueButton.SetActive(false);
			SkipTutorialButton.SetActive(false);
			FinishButton.SetActive(true);
			TutorialText.text = "Good luck and have fun walking your dog!";
			waitingForPlayer = true;
			while (waitingForPlayer)
			{
				yield return null;
			}
			gameManager.TogglePause();
			RestartButton.interactable = true;
			TutorialScreen.SetActive(false);
		}
	}
}