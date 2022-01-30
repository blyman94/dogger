using Dogger.Control;
using Dogger.Core;
using Dogger.Player;
using Dogger.Spawning;
using Dogger.UI;
using NSubstitute;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ui_manager
    {
        [UnityTest]
        public IEnumerator alerts_game_manager_when_player_health_is_zero()
        {
            //Arrange
            CreateUIManager(out GameObject uiManagerObject,
                out UIManager uiManager);

            CreateGameManager(out GameObject gameManagerObject, 
                out GameManager gameManager);
            gameManager.Testing = true;

            CreatePlayer(out GameObject playerObject, 
                out PlayerCharacter player);

            // Act
            player.Health.Decrement(player.Health.Current);
            yield return null;

            // Assert
            Assert.IsTrue(gameManager.IsGameOver);

            // Clean
            uiManagerObject.SetActive(false);
            playerObject.SetActive(false);
            gameManagerObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator updates_player_coins_when_signaled_by_player()
        {
            // Arrange
            CreateUIManager(out GameObject uiManagerObject,
                out UIManager uiManager);
            CreatePlayer(out GameObject playerObject,
                out PlayerCharacter player);

            GameObject coinObject = new GameObject("Coin");
            coinObject.AddComponent<Obstacle>();
            coinObject.transform.tag = "Coin";
            coinObject.transform.position = Vector3.forward;

            // Act
            coinObject.GetComponent<Rigidbody>().AddForce(Vector3.back,
                ForceMode.VelocityChange);
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.AreEqual(1, uiManager.hudManager.PlayerCoins);

            // Clean
            uiManagerObject.SetActive(false);
            playerObject.SetActive(false);
            coinObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator updates_player_health_when_signaled_by_player()
        {
            // Arrange
            CreateUIManager(out GameObject uiManagerObject, 
                out UIManager uiManager);
            CreatePlayer(out GameObject playerObject, 
                out PlayerCharacter player);
            CreateObstacle(out GameObject obstacleObject);

            // Act
            obstacleObject.GetComponent<Rigidbody>().AddForce(Vector3.back,
                ForceMode.VelocityChange);
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.AreEqual(2, uiManager.hudManager.PlayerHealth);

            // Clean
            uiManagerObject.SetActive(false);
            playerObject.SetActive(false);
            obstacleObject.SetActive(false);
        }
        
        [UnityTest]
        public IEnumerator updates_player_score_according_to_spawn_manager_speed()
        {
            // Arrange
            CreateUIManager(out GameObject uiManagerObject, 
                out UIManager uiManager);
            uiManager.hudManager.PlayerScore = 0;
            Spawner.CurrentSpeed = 1.0f;
            CreateSpawnManager(out GameObject spawnManagerObject, 
                out SpawnManager spawnManager);

            // Act
            yield return new WaitForSeconds(1f);

            // Assert
            Assert.AreEqual(1, uiManager.hudManager.PlayerScore, 0.1f);

            // Clean
            uiManagerObject.SetActive(false);
            spawnManagerObject.SetActive(false);
        }

        private void CreateGameManager(out GameObject gameManagerObject,
            out GameManager gameManager)
        {
            gameManagerObject = new GameObject("Game Manager");
            gameManager = gameManagerObject.AddComponent<GameManager>();
        }

        private void CreateObstacle(out GameObject obstacleObject)
        {
            obstacleObject = new GameObject("Obstacle");
            obstacleObject.transform.tag = "Obstacle";
            obstacleObject.transform.position = new Vector3(0.0f, 0.0f, 1.0f);
            obstacleObject.AddComponent<Obstacle>();
            Rigidbody rb = obstacleObject.GetComponent<Rigidbody>();
            rb.useGravity = false;
        }

        private void CreatePlayer(out GameObject playerObject,
            out PlayerCharacter player)
        {
            playerObject = new GameObject("Player");
            playerObject.transform.position = Vector3.zero;
            player = playerObject.AddComponent<PlayerCharacter>();
            player.CollisionHandler = 
                playerObject.AddComponent<PlayerCollisionHandler>();
            player.PlayerInput = Substitute.For<IPlayerInput>();
        }

        private void CreateSpawnManager(out GameObject spawnManagerObject, 
            out SpawnManager spawnManager)
        {
            spawnManagerObject = new GameObject("Spawn Manager");
            spawnManager = spawnManagerObject.AddComponent<SpawnManager>();
        }

        private void CreateUIManager(out GameObject uiManagerObject,
            out UIManager uiManager)
        {
            uiManagerObject = new GameObject("UI Manager");
            uiManager = uiManagerObject.AddComponent<UIManager>();
            uiManager.hudManager = uiManagerObject.AddComponent<HUDManager>();
        }
    }
}