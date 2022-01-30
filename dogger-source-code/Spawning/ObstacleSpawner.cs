using Dogger.Dog;
using UnityEngine;

namespace Dogger.Spawning
{
	/// <summary>
	/// Spawns dog waste, trash, benches, planters, double light posts and full
	/// light posts in the traversable player area.
	/// </summary>
	public class ObstacleSpawner : Spawner
	{
		/// <summary>
		/// Dog for the spawner to pass focus objects to.
		/// </summary>
		public DogCharacter Dog;

		protected override void Start()
		{
			base.Start();
			InvokeRepeating("RandomObstacleSpawn", SpawnData.SpawnStartTime, SpawnRate);
		}

		/// <summary>
		/// Chooses a random obstacle to spawn (dog waste, trash, bench, 
		/// planter, double light post, full light post) and spawns it in the
		/// traversable player area.
		/// </summary>
		public void RandomObstacleSpawn()
		{
			// Randomly choose object to spawn
			int randomObject = Random.Range(0, Pools.Length);

			// Randomly choose position to spawn
			float randomXPos = Random.Range(Obstacle.BoundXMin, Obstacle.BoundXMax);

			GameObject objectToSpawn = GetObjectFromPool(randomObject);
			if (objectToSpawn != null)
			{
				objectToSpawn.transform.position = new Vector3(randomXPos, transform.position.y, transform.position.z);
				objectToSpawn.SetActive(true);
				Rigidbody objectRb = objectToSpawn.GetComponent<Rigidbody>();
				if (objectRb != null)
				{
					objectRb.AddForce(Vector3.back * CurrentSpeed, ForceMode.VelocityChange);

					if (Dog != null)
					{
						Dog.AddObjectToFocusQueue(objectToSpawn);
					}
					NumberSpawned += 1;
				}
				else
				{
					Debug.LogError("ObstacleSpawner.cs :: Tried to spawn object without a Rigidbody.");
				}
			}
		}
	}
}