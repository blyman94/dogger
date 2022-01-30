using UnityEngine;

namespace Dogger.Spawning
{
	/// <summary>
	/// Randomly spawns bushes, grass, and other objects to further sell the 
	/// appearance that the player is moving forward.
	/// </summary>
	public class ScenerySpawner : Spawner
	{
		/// <summary>
		/// Maximum wait time between objects spawning.
		/// </summary>
		public float maxSpawnRate;

		/// <summary>
		/// Maximum x position of spawned scenery so as not to intersect the
		/// traversable player area (road) or move too far out of the camera
		/// (grass).
		/// </summary>
		public float maxXPos;

		/// <summary>
		/// Minimum wait time between objects spawning.
		/// </summary>
		public float minSpawnRate;

		/// <summary>
		/// Minimum x position of spawned scenery so as not to intersect the
		/// traversable player area (grass) or move too far out of the camera
		/// (road).
		/// </summary>
		public float minXPos;

		protected override void Start()
		{
			base.Start();
			Invoke("RandomScenerySpawn", 0.0f);
		}

		/// <summary>
		/// Spawns a random scenery object to scroll toward the player.
		/// </summary>
		private void RandomScenerySpawn()
		{
			// Randomly choose object to spawn
			int randomObject = Random.Range(0, Pools.Length);

			// Randomly choose position to spawn
			float randomXPos = Random.Range(minXPos, maxXPos);

			GameObject objectToSpawn = GetObjectFromPool(randomObject);

			if (objectToSpawn != null)
			{
				objectToSpawn.transform.position = new Vector3(randomXPos,
					objectToSpawn.transform.position.y, transform.position.z);
				objectToSpawn.SetActive(true);
				Rigidbody objectRb = objectToSpawn.GetComponent<Rigidbody>();
				if (objectRb != null)
				{
					objectRb.AddForce(Vector3.back * CurrentSpeed,
						ForceMode.VelocityChange);
					NumberSpawned += 1;
				}
				else
				{
					Debug.LogError("ScenerySpawner.cs :: Tried to spawn " +
						"object without a Rigidbody.");
				}
			}

			Invoke("RandomScenerySpawn", Random.Range(minSpawnRate, maxSpawnRate));
		}
	}
}

