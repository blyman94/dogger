using RopeMinikit;
using System.Collections;
using UnityEngine;

namespace Dogger.Player
{
	/// <summary>
	/// Handles all collisions for the player.
	/// </summary>
	public class PlayerCollisionHandler : MonoBehaviour
	{
		/// <summary>
		/// Tracks whether the player is currently debuffed. A debuff reverses
		/// the player's horizontal controls.
		/// </summary>
		[HideInInspector] public bool IsDebuffed;

		/// <summary>
		/// The associated player.
		/// </summary>
		[SerializeField] private PlayerCharacter player;

		/// <summary>
		/// The collider of the associated player.
		/// </summary>
		[SerializeField] private CapsuleCollider playerCollider;

		public delegate void CollidedWithObstacle(int health);
		public static CollidedWithObstacle collidedWithObstacle;
		public delegate void CollidedWithCoin();
		public static CollidedWithCoin collidedWithCoin;
		public delegate void CollidedWithWaste();
		public static CollidedWithWaste collidedWithWaste;
		public delegate void DebuffEnded();
		public static DebuffEnded debuffEnded;

		public void Awake()
		{
			if (player == null)
			{
				player = GetComponent<PlayerCharacter>();
			}
			if (playerCollider == null)
			{
				playerCollider = GetComponent<CapsuleCollider>();
			}
			playerCollider.isTrigger = true;
		}

		private void OnEnable()
		{
			Rope.CollidedWithPoleObject += ReactToRopePoleCollision;
		}

		private void OnDisable()
		{
			Rope.CollidedWithPoleObject -= ReactToRopePoleCollision;
		}

		private void OnTriggerEnter(Collider other)
		{
			switch (other.transform.tag)
			{
				case "Coin":
					RespondToCoinCollision(other);
					break;
				case "Waste":
					RespondToWasteCollision();
					break;
				case "Obstacle":
					RespondToObstacleCollision();
					break;
				case "Pole":
					RespondToObstacleCollision();
					break;
				case "Default":
					Debug.LogError("PlayerCollisionHandler.cs :: Player" +
						"collided with unregcognized object.");
					break;
			}
		}

		/// <summary>
		/// A routine that deactivates a debuff three seconds after it is
		/// obtained.
		/// </summary>
		private IEnumerator DebuffTimerRoutine()
		{
			yield return new WaitForSeconds(3.0f);
			IsDebuffed = false;
			debuffEnded?.Invoke();
		}

		/// <summary>
		/// Checks to see if the pole has truly broken the plane between
		/// player and dog upon rope contact with pole, and ends the game if 
		/// so.
		/// </summary>
		/// <param name="collisionPoint">The point of the pole with which
		/// the rope collided.</param>
		private void ReactToRopePoleCollision(Vector3 collisionPoint)
		{
			float playerX = player.gameObject.transform.position.x;
			float dogX = player.Dog.gameObject.transform.position.x;
			float dogZ = player.Dog.gameObject.transform.position.z;
			if (((collisionPoint.x > playerX + 0.1f && collisionPoint.x < dogX - 0.1f) ||
				(collisionPoint.x < playerX - 0.1f && collisionPoint.x > dogX + 0.1f)) &&
				collisionPoint.z < dogZ)
			{
				player.Health.Decrement(player.Health.Current);
			}
		}

		private void RespondToCoinCollision(Collider coin)
		{
			player.Bank.AddCoin(1);
			coin.gameObject.SetActive(false);
			collidedWithCoin?.Invoke();
		}

		private void RespondToObstacleCollision()
		{
			player.Health.Decrement(1);
			collidedWithObstacle?.Invoke(player.Health.Current);
		}

		private void RespondToWasteCollision()
		{
			IsDebuffed = true;
			collidedWithWaste?.Invoke();
			StartCoroutine(DebuffTimerRoutine());
		}
	}
}