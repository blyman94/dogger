using UnityEngine;

namespace Dogger.Spawning
{
	/// <summary>
	/// Component deactivates the object if it passes behind the player.
	/// </summary>
	[RequireComponent(typeof(Rigidbody))]
	public class Scroller : MonoBehaviour
	{
		public Rigidbody ScrollerRb;

		protected virtual void Awake()
		{
			if (ScrollerRb != null)
			{
				ScrollerRb.useGravity = false;
			}
		}

		protected virtual void Update()
		{
			DeactivateIfGoesOffScreen();
			SpinCoin();
		}

		/// <summary>
		/// Deactivates the object if it scrolls too far past the player along
		/// the z axis.
		/// </summary>
		private void DeactivateIfGoesOffScreen()
		{
			if (transform.position.z < -2.0f)
			{
				if (ScrollerRb != null)
				{
					ScrollerRb.velocity = Vector3.zero;
				}
				gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// If the spawned object is a coin, rotate it to add visual flare.
		/// </summary>
		private void SpinCoin()
		{
			if (transform.CompareTag("Coin"))
			{
				transform.Rotate(0.0f, 100.0f * Time.deltaTime, 0.0f);
			}
		}
	}
}