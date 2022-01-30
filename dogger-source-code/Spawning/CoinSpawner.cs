using UnityEngine;

namespace Dogger.Spawning
{
	/// <summary>
	/// Class to spawn coins in the traversable player area. Keeping the spawn 
	/// rates between coins and obstacles different will lead to more
	/// interesting gameplay.
	/// </summary>
	public class CoinSpawner : Spawner
	{
		protected override void Start()
		{
			base.Start();
			InvokeRepeating("RandomCoinSpawn", SpawnData.SpawnStartTime, SpawnRate);
		}

		/// <summary>
		/// Spawns a coin at a random position within the traversable player 
		/// area.
		/// </summary>
		public void RandomCoinSpawn()
		{
			// Randomly choose position to spawn
			float randomXPos = Random.Range(Obstacle.BoundXMin, Obstacle.BoundXMax);

			GameObject coinToSpawn = GetObjectFromPool(0);
			if (coinToSpawn != null)
			{
				coinToSpawn.transform.position = new Vector3(randomXPos, 0.0f, transform.position.z);
				coinToSpawn.SetActive(true);
				Rigidbody coinRb = coinToSpawn.GetComponent<Rigidbody>();
				if (coinRb != null)
				{
					coinRb.AddForce(Vector3.back * CurrentSpeed, ForceMode.VelocityChange);
					NumberSpawned += 1;
				}
				else
				{
					Debug.LogError("CoinSpawner.cs :: Tried to spawn object without a Rigidbody.");
				}
			}
		}
	}
}

