using UnityEngine;

namespace Dogger.Spawning
{
	/// <summary>
	/// Stores pool size, prefab array, and spawn start time for each spawner.
	/// </summary>
	[System.Serializable]
	public struct SpawnData
	{
		public int PoolSize;
		public GameObject[] Prefabs;
		public float SpawnStartTime;
	}

	/// <summary>
	/// Parent class for all spawner objects. Employs object pooling for
	/// efficient storage of randomly spawned objects.
	/// </summary>
	public abstract class Spawner : MonoBehaviour
	{
		public SpawnData SpawnData;

		/// <summary>
		/// Global current speed setting for all spawners. Helps to ensure that
		/// all spawned objects in the game move at the same rate, giving the 
		/// appearance that the player is moving forward.
		/// </summary>
		public static float CurrentSpeed { get; set; }

		/// <summary>
		/// Counter for number of objects spawned used for unit tests.
		/// </summary>
		public int NumberSpawned { get; set; }

		/// <summary>
		/// An array that stores object pools for each object in the
		/// SpawnData's prefab list.
		/// </summary>
		public GameObject[][] Pools { get; set; }

		/// <summary>
		/// Wait time between object spawns.
		/// </summary>
		public float SpawnRate { get; set; }

		protected virtual void Start()
		{
			if (SpawnData.Prefabs != null)
			{
				Pools = new GameObject[SpawnData.Prefabs.Length][];
				InitializeObjectPools();
			}
		}

		/// <summary>
		/// Returns an object from the pool specified by objectIndex for
		/// spawning.
		/// </summary>
		/// <param name="objectIndex">Object type to spawn.</param>
		/// <returns>An object to be spawned</returns>
		protected GameObject GetObjectFromPool(int objectIndex)
		{
			if (Pools != null)
			{
				GameObject[] prefabPool = Pools[objectIndex];

				for (int i = 0; i < prefabPool.Length; i++)
				{
					if (!prefabPool[i].activeInHierarchy)
					{
						prefabPool[i].GetComponent<Rigidbody>().Sleep();
						return prefabPool[i];
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Creates object pools based on the SpawnData's pool size.
		/// </summary>
		private void InitializeObjectPools()
		{
			for (int i = 0; i < SpawnData.Prefabs.Length; i++)
			{
				Pools[i] = new GameObject[SpawnData.PoolSize];
				for (int j = 0; j < SpawnData.PoolSize; j++)
				{
					Pools[i][j] = Instantiate(SpawnData.Prefabs[i], transform);
					Pools[i][j].SetActive(false);
				}
			}
		}
	}
}