using Dogger.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dogger.Spawning
{
    /// <summary>
    /// Manages all spawners in the tutorial or main scene.
    /// </summary>
    public class SpawnManager : MonoBehaviour
    {
        public CoinSpawner CoinSpawner;
        public float CurrentSpeed;
        public ObstacleSpawner ObstacleSpawner;
        public ScenerySpawner ScenerySpawnerGrass;
        public ScenerySpawner ScenerySpawnerRoad;
        public SessionPreferences_SO sessionPrefs;

        public delegate void CurrentSpeedChanged(float currentSpeed);
        public static CurrentSpeedChanged currentSpeedChanged;

        public void Awake()
        {
            if (sessionPrefs != null)
            {
                if (SceneManager.GetActiveScene().name == "Tutorial")
                {
                    Spawner.CurrentSpeed = 2.0f;
                }
                else
                {
                    SetSpawnRateBasedOnDifficulty();
                    SetSpawnerSpeedBasedOnDifficulty();
                }

            }
            currentSpeedChanged?.Invoke(Spawner.CurrentSpeed);
        }

        private void SetSpawnerSpeedBasedOnDifficulty()
        {
            switch (sessionPrefs.Difficulty)
            {
                case 0:
                    Spawner.CurrentSpeed = 2.0f;
                    break;
                case 1:
                    Spawner.CurrentSpeed = 4.0f;
                    break;
                case 2:
                    Spawner.CurrentSpeed = 6.0f;
                    break;
            }
        }

        private void SetSpawnRateBasedOnDifficulty()
        {
            switch (sessionPrefs.Difficulty)
            {
                case 0:
                    if (CoinSpawner != null)
                    {
                        CoinSpawner.SpawnRate = 4.5f;
                    }
                    if (ObstacleSpawner != null)
                    {
                        ObstacleSpawner.SpawnRate = 4.0f;
                    }
                    break;
                case 1:
                    if (CoinSpawner != null)
                    {
                        CoinSpawner.SpawnRate = 2.5f;
                    }
                    if (ObstacleSpawner != null)
                    {
                        ObstacleSpawner.SpawnRate = 2.0f;
                    }
                    break;
                case 2:
                    if (CoinSpawner != null)
                    {
                        CoinSpawner.SpawnRate = 1.3f;
                    }
                    if (ObstacleSpawner != null)
                    {
                        ObstacleSpawner.SpawnRate = 0.8f;
                    }
                    break;
            }
        }
    }
}

