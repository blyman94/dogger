using Dogger.Player;
using System.Collections;
using UnityEngine;

namespace Dogger.Control
{
	/// <summary>
	/// Shakes the screen when the player collides with an obstacle.
	/// </summary>
	public class CameraController : MonoBehaviour
	{
		/// <summary>
		/// Duration of the camera shake.
		/// </summary>
		public float shakeDuration;

		/// <summary>
		/// Magnitude of screen position deviation during shake.
		/// </summary>
		public float shakeIntensity;

		/// <summary>
		/// Position of the camera before any shaking.
		/// </summary>
		private Vector3 startPos;

		private void Awake()
		{
			startPos = transform.position;
		}

		private void OnEnable()
		{
			PlayerCollisionHandler.collidedWithObstacle += CameraShake;
		}

		private void OnDisable()
		{
			PlayerCollisionHandler.collidedWithObstacle -= CameraShake;
		}

		/// <summary>
		/// Starts the camera shake routine if player health is greater than 0.
		/// </summary>
		/// <param name="health">Player health remaining after object 
		/// collision.</param>
		private void CameraShake(int health)
		{
			if (health > 0)
			{
				StartCoroutine(CameraShakeRoutine());
			}
		}

		/// <summary>
		/// Coroutine to shake the camera driven by Random.insideUnitSphere.
		/// </summary>
		/// <returns>Yield returns null over the shake duration.</returns>
		private IEnumerator CameraShakeRoutine()
		{
			float elapsedTime = 0;
			while (elapsedTime < shakeDuration)
			{
				transform.position = startPos + (Random.insideUnitSphere * 
					shakeIntensity);
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			transform.position = startPos;
		}
	}
}