using Dogger.Core;
using UnityEngine;
using Dogger.UI;

namespace Dogger.Dog
{
	/// <summary>
	/// Visually displays dogs to the player so that the player may choose a 
	/// dog to walk in game.
	/// </summary>
	public class DogSelector : MonoBehaviour
	{
		/// <summary>
		/// Array of DogAvatar objects to visually represent the dog the player
		/// is choosing.
		/// </summary>
		public GameObject[] DogAvatars;

		/// <summary>
		/// Array of strings representing the names of the available dogs.
		/// </summary>
		public string[] DogNames;

		/// <summary>
		/// Player session preferences scriptable object which will store the
		/// selected dog's ID number and name.
		/// </summary>
		public SessionPreferences_SO sessionPrefs;

		private bool dogIsShown;

		public delegate void DogNameChanged(string newName);
		public static DogNameChanged dogNameChanged;

		public void OnEnable()
		{
			ScreenManager.switchedToDogSelect += ShowDog;
			ScreenManager.switchedFromDogSelect += HideDog;
			UIManager.nextDog += NextDog;
			UIManager.prevDog += PreviousDog;
		}

		public void Start()
		{
			foreach (GameObject dogObject in DogAvatars)
			{
				dogObject.SetActive(false);
			}
		}

		public void OnDisable()
		{
			ScreenManager.switchedToDogSelect -= ShowDog;
			ScreenManager.switchedFromDogSelect -= HideDog;
			UIManager.nextDog -= NextDog;
			UIManager.prevDog -= PreviousDog;
		}

		/// <summary>
		/// Shows the next dog in the DogAvatars array and changes the session
		/// preferences accordingly.
		/// </summary>
		public void NextDog()
		{
			if (dogIsShown)
			{
				DogAvatars[sessionPrefs.DogID].SetActive(false);
				if (sessionPrefs.DogID + 1 > DogAvatars.Length - 1)
				{
					sessionPrefs.DogID = 0;
				}
				else
				{
					sessionPrefs.DogID += 1;
				}
				sessionPrefs.DogName = DogNames[sessionPrefs.DogID];
				DogAvatars[sessionPrefs.DogID].SetActive(true);
				dogNameChanged?.Invoke(DogNames[sessionPrefs.DogID]);
			}
		}

		/// <summary>
		/// Shows the previous dog in the DogAvatars array and changes the 
		/// session preferences accordingly.
		/// </summary>
		public void PreviousDog()
		{
			if (dogIsShown)
			{
				DogAvatars[sessionPrefs.DogID].SetActive(false);
				if (sessionPrefs.DogID - 1 < 0)
				{
					sessionPrefs.DogID = DogAvatars.Length - 1;
				}
				else
				{
					sessionPrefs.DogID -= 1;
				}
				sessionPrefs.DogName = DogNames[sessionPrefs.DogID];
				DogAvatars[sessionPrefs.DogID].SetActive(true);
				dogNameChanged?.Invoke(DogNames[sessionPrefs.DogID]);
			}
		}
		
		/// <summary>
		/// Hides the current DogAvatar object for when the player is not on 
		/// the dog selection screen.
		/// </summary>
		private void HideDog()
		{
			dogIsShown = false;
			DogAvatars[sessionPrefs.DogID].SetActive(false);
		}

		/// <summary>
		/// Shows the current DogAvatar object for when the player is on the 
		/// dog selection screen.
		/// </summary>
		private void ShowDog()
		{
			dogIsShown = true;
			DogAvatars[sessionPrefs.DogID].SetActive(true);
			sessionPrefs.DogName = DogNames[sessionPrefs.DogID];
			dogNameChanged?.Invoke(DogNames[sessionPrefs.DogID]);
		}
	}
}

