using Dogger.Dog;
using Dogger.Spawning;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class spawn_manager
    {
        [UnityTest]
        public IEnumerator creates_three_obstacle_pools_when_given_three_prefabs()
        {
            // Arrange
            GameObject spawnManagerObject = new GameObject("Spawn Manager");
            SpawnManager spawnManager = 
                spawnManagerObject.AddComponent<SpawnManager>();
            ObstacleSpawner obstacleSpawner = 
                spawnManagerObject.AddComponent<ObstacleSpawner>();
            spawnManager.ObstacleSpawner = obstacleSpawner;
            spawnManager.ObstacleSpawner.SpawnData.Prefabs = new GameObject[3];

            spawnManager.ObstacleSpawner.SpawnData.Prefabs[0] = 
                CreateObstacle(Vector3.zero);
            spawnManager.ObstacleSpawner.SpawnData.Prefabs[1] = 
                CreatePole(Vector3.zero);
            spawnManager.ObstacleSpawner.SpawnData.Prefabs[2] = 
                CreateWaste(Vector3.zero);

            // Act
            yield return new WaitForSeconds(0.3f);

            // Assert
            Assert.IsTrue(spawnManager.ObstacleSpawner.Pools[0] != null);
            Assert.IsTrue(spawnManager.ObstacleSpawner.Pools[1] != null);
            Assert.IsTrue(spawnManager.ObstacleSpawner.Pools[2] != null);

            // Clean
            spawnManagerObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator instantiates_30_pool_objects_when_given_three_prefabs_and_10_prefab_pool_size()
        {
            // Arrange
            GameObject spawnManagerObject = new GameObject("Spawn Manager");
            SpawnManager spawnManager = 
                spawnManagerObject.AddComponent<SpawnManager>();
            ObstacleSpawner obstacleSpawner = 
                spawnManagerObject.AddComponent<ObstacleSpawner>();
            spawnManager.ObstacleSpawner = obstacleSpawner;
            spawnManager.ObstacleSpawner.SpawnData.Prefabs = new GameObject[3];
            spawnManager.ObstacleSpawner.SpawnData.PoolSize = 10;

            spawnManager.ObstacleSpawner.SpawnData.Prefabs[0] = 
                CreateObstacle(Vector3.zero);
            spawnManager.ObstacleSpawner.SpawnData.Prefabs[1] = 
                CreatePole(Vector3.zero);
            spawnManager.ObstacleSpawner.SpawnData.Prefabs[2] = 
                CreateWaste(Vector3.zero);

            // Act
            yield return new WaitForSeconds(0.3f);

            // Assert
            Assert.AreEqual(30, spawnManager.transform.childCount);

            // Clean
            spawnManagerObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator spawned_obstacle_is_passed_to_dog_focus_queue()
        {
            // Arrange
            GameObject spawnManagerObject = new GameObject("Spawn Manager");
            SpawnManager spawnManager = 
                spawnManagerObject.AddComponent<SpawnManager>();
            ObstacleSpawner obstacleSpawner = 
                spawnManagerObject.AddComponent<ObstacleSpawner>();
            spawnManager.ObstacleSpawner = obstacleSpawner;
            spawnManager.ObstacleSpawner.SpawnData.PoolSize = 3;
            spawnManager.ObstacleSpawner.SpawnData.Prefabs = new GameObject[1];
            spawnManager.ObstacleSpawner.SpawnData.Prefabs[0] = 
                CreateObstacle(Vector3.zero);

            GameObject dogGameObject = new GameObject("Dog");
            dogGameObject.transform.position = Vector3.zero;
            DogCharacter dog = dogGameObject.AddComponent<DogCharacter>();

            spawnManager.ObstacleSpawner.Dog = dog;

            // Act
            yield return new WaitForSeconds(0.3f);
            spawnManager.ObstacleSpawner.RandomObstacleSpawn();

            // Assert
            Assert.AreEqual("Obstacle", dog.CurrentObject.transform.tag);

            // Clean
            spawnManagerObject.SetActive(false);
            dogGameObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator spawns_coins_at_coin_spawn_rate()
        {
            // Arrange
            GameObject spawnManagerObject = new GameObject("SpawnManager");
            SpawnManager spawnManager = 
                spawnManagerObject.AddComponent<SpawnManager>();
            CoinSpawner coinSpawner = 
                spawnManagerObject.AddComponent<CoinSpawner>();
            spawnManager.CoinSpawner = coinSpawner;
            spawnManager.CoinSpawner.SpawnRate = 0.30f;
            spawnManager.CoinSpawner.SpawnData.SpawnStartTime = 0.0f;
            spawnManager.CoinSpawner.SpawnData.PoolSize = 5;

            GameObject coinGameObject = new GameObject("Coin");
            coinGameObject.transform.tag = "Coin";
            coinGameObject.AddComponent<Obstacle>();

            spawnManager.CoinSpawner.SpawnData.Prefabs = new GameObject[1];
            spawnManager.CoinSpawner.SpawnData.Prefabs[0] = coinGameObject;

            // Act
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.AreEqual(4, spawnManager.CoinSpawner.NumberSpawned);

            // Clean
            spawnManagerObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator spawns_objects_at_spawn_rate()
        {
            // Arrange
            GameObject spawnManagerObject = new GameObject("SpawnManager");
            SpawnManager spawnManager = 
                spawnManagerObject.AddComponent<SpawnManager>();
            ObstacleSpawner obstacleSpawner = 
                spawnManagerObject.AddComponent<ObstacleSpawner>();
            spawnManager.ObstacleSpawner = obstacleSpawner;
            spawnManager.ObstacleSpawner.SpawnRate = 0.30f;
            spawnManager.ObstacleSpawner.SpawnData.SpawnStartTime = 0.0f;
            spawnManager.ObstacleSpawner.SpawnData.PoolSize = 5;

            spawnManager.ObstacleSpawner.SpawnData.Prefabs = new GameObject[1];
            spawnManager.ObstacleSpawner.SpawnData.Prefabs[0] = 
                CreateObstacle(Vector3.zero);

            // Act
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.AreEqual(4, spawnManager.ObstacleSpawner.NumberSpawned);

            // Clean
            spawnManagerObject.SetActive(false);
        }
        private static GameObject CreateObstacle(Vector3 spawnPos)
        {
            GameObject obstacleGameObject = new GameObject("Obstacle");
            obstacleGameObject.transform.tag = "Obstacle";
            obstacleGameObject.layer = 9;
            obstacleGameObject.transform.position = spawnPos;
            obstacleGameObject.AddComponent<Obstacle>();
            return obstacleGameObject;
        }

        private static GameObject CreatePole(Vector3 spawnPos)
        {
            GameObject poleGameObject = new GameObject("Pole");
            poleGameObject.transform.tag = "Pole";
            poleGameObject.transform.position = spawnPos;
            poleGameObject.AddComponent<Obstacle>();
            return poleGameObject;
        }

        private static GameObject CreateWaste(Vector3 spawnPos)
        {
            GameObject wasteGameObject = new GameObject("Waste");
            wasteGameObject.transform.tag = "Waste";
            wasteGameObject.transform.position = spawnPos;
            wasteGameObject.AddComponent<Obstacle>();
            return wasteGameObject;
        }
    }
}