using UnityEngine;

namespace Dogger.Player
{
	/// <summary>
	/// Handles all player audio in reaction to game events.
	/// </summary>
	public class PlayerAudio : MonoBehaviour
	{
		[Header("Audio Sources")]
		[Space]
		[SerializeField] private AudioSource coinPickupSource;
		[SerializeField] private AudioSource footSource;
		[SerializeField] private AudioSource obstacleHitSource;

		[Header("Audio Clips")]
		[Space]
		[SerializeField] private AudioClip coinClip;
		[SerializeField] private AudioClip footstepClip;
		[SerializeField] private AudioClip obstacleClip;
		[SerializeField] private AudioClip wasteClip;

		private void OnEnable()
		{
			PlayerCollisionHandler.collidedWithCoin += PlayCoinPickupClip;
			PlayerCollisionHandler.collidedWithObstacle += PlayObstacleHitClip;
			PlayerCollisionHandler.collidedWithWaste += PlayWasteStepClip;
		}

		private void OnDisable()
		{
			PlayerCollisionHandler.collidedWithCoin -= PlayCoinPickupClip;
			PlayerCollisionHandler.collidedWithObstacle -= PlayObstacleHitClip;
			PlayerCollisionHandler.collidedWithWaste -= PlayWasteStepClip;
		}

		private void PlayCoinPickupClip()
		{
			if (!coinPickupSource.isPlaying)
			{
				coinPickupSource.PlayOneShot(coinClip, 1.0f);
			}
		}

		/// <summary>
		/// Responds to "PlayFootstep" animation event.
		/// </summary>
		public void PlayFootstep()
		{
			if (!footSource.isPlaying)
			{
				footSource.pitch = Random.Range(0.8f, 1.2f);
				footSource.PlayOneShot(footstepClip, 1.0f);
			}
		}

		private void PlayObstacleHitClip(int health)
		{
			if (!obstacleHitSource.isPlaying)
			{
				obstacleHitSource.pitch = Random.Range(0.8f, 1.2f);
				obstacleHitSource.PlayOneShot(obstacleClip, 1.0f);
			}
		}

		private void PlayWasteStepClip()
		{
			if (!obstacleHitSource.isPlaying)
			{
				obstacleHitSource.PlayOneShot(wasteClip, 1.0f);
			}
		}
	}
}

