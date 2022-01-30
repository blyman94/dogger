using UnityEngine;

namespace Dogger.Dog
{
	/// <summary>
	/// Short enum to streamline dog size selection in the editor. Possible 
	/// values: Small, Large
	/// </summary>
	public enum DogSize { Small, Large }

	/// <summary>
	/// Handles all audio (barking and waste pickup) for the dog.
	/// </summary>
	public class DogAudio : MonoBehaviour
	{
		public AudioSource DogAudioSource;

		/// <summary>
		/// Dictates the size of the dog, and therefore its type of bark.
		/// </summary>
		public DogSize DogSize;

		/// <summary>
		/// Bark audio clip to play for large dogs.
		/// </summary>
		public AudioClip largeBarkClip;

		/// <summary>
		/// Bark audio clip to play for small dogs.
		/// </summary>
		public AudioClip smallBarkClip;

		/// <summary>
		/// Clip to play when dog picks up waste.
		/// </summary>
		public AudioClip wasteClip;

		private void OnEnable()
		{
			DogCharacter.pickedUpWaste += PlayWasteClip;
		}

		private void Start()
		{
			Invoke("Bark", 5.0f);
		}

		private void OnDisable()
		{
			DogCharacter.pickedUpWaste -= PlayWasteClip;
		}

		/// <summary>
		/// Determines which bark audio clip to play based on dog size, then 
		/// plays the bark audio clip at random intervals between 10 and 35
		/// seconds.
		/// </summary>
		private void Bark()
		{
			if (DogSize == DogSize.Small)
			{
				DogAudioSource.pitch = Random.Range(1.0f, 1.25f);
				DogAudioSource.PlayOneShot(smallBarkClip);
			}
			else
			{
				DogAudioSource.pitch = Random.Range(0.75f, 1.0f);
				DogAudioSource.PlayOneShot(largeBarkClip);
			}

			Invoke("Bark", Random.Range(10.0f, 35.0f));
		}

		/// <summary>
		/// Plays the waste pickup clip if the player's health is greater than
		/// 0.
		/// </summary>
		/// <param name="health">
		/// The player's remaining health after the
		/// dog picks up the waste.
		/// </param>
		private void PlayWasteClip(int health)
		{
			if (health > 0)
			{
				DogAudioSource.pitch = Random.Range(0.5f, 0.6f);
				DogAudioSource.PlayOneShot(wasteClip, 10.0f);
			}
		}
	}
}