using Dogger.Control;
using Dogger.Dog;
using Dogger.Player;
using Dogger.Spawning;
using NSubstitute;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;


namespace Tests
{
    public class player
    {
        [UnityTest]
        public IEnumerator click_and_drag_left_moves_dog_left_when_dog_is_right()
        {
            // Arrange
            CreateCamera(out GameObject cameraObject);
            CreatePlayer(out GameObject playerObject,
                out PlayerCharacter player);
            CreateDog(out GameObject dogObject, out DogCharacter dog,
                new Vector3(2.0f, 0.0f, 2.0f));
            CreateLeash(out GameObject leashObject,
                out Leash leash, player, dog);

            player.Dog = dog;
            player.Leash = leash;
            dog.Player = player;

            // Act
            Vector3 playerScreenPos = 
                Camera.main.WorldToScreenPoint(playerObject.transform.position);
            player.PlayerInput.MousePosition.Returns(playerScreenPos);
            player.PlayerInput.MouseLeftClicked.Returns(true);
            yield return null;
            player.PlayerInput.MousePosition.Returns(playerScreenPos -
                new Vector3(5.0f, 0.0f, 0.0f));
            yield return new WaitForSeconds(1.0f);
            player.PlayerInput.MouseLeftClicked.Returns(false);

            // Assert
            Assert.IsTrue(2.0f > dogObject.transform.position.x);

            // Clean
            cameraObject.SetActive(false);
            playerObject.SetActive(false);
            dogObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator click_and_drag_right_moves_dog_right_when_dog_is_left()
        {
            // Arrange
            CreateCamera(out GameObject cameraObject);
            CreatePlayer(out GameObject playerObject, 
                out PlayerCharacter player);
            CreateDog(out GameObject dogObject, out DogCharacter dog,
                new Vector3(-2.0f, 0.0f, 2.0f));
            CreateLeash(out GameObject leashObject, 
                out Leash leash, player, dog);

            player.Dog = dog;
            player.Leash = leash;
            dog.Player = player;

            // Act
            Vector3 playerScreenPos = 
                Camera.main.WorldToScreenPoint(playerObject.transform.position);
            player.PlayerInput.MousePosition.Returns(playerScreenPos);
            player.PlayerInput.MouseLeftClicked.Returns(true);
            yield return null;
            player.PlayerInput.MousePosition.Returns(playerScreenPos + 
                new Vector3(5.0f, 0.0f, 0.0f));
            yield return new WaitForSeconds(1.0f);
            player.PlayerInput.MouseLeftClicked.Returns(false);

            // Assert
            Assert.IsTrue(-2.0f < dogObject.transform.position.x);

            // Clean
            cameraObject.SetActive(false);
            playerObject.SetActive(false);
            dogObject.SetActive(false);
            leashObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator contact_with_obstacle_reduces_health_by_one()
        {
            //Arrange
            CreatePlayer(out GameObject playerObject,
                out PlayerCharacter player);
            GameObject obstacleGameObject = CreateObstacle();

            // Act
            player.PlayerInput.Horizontal.Returns(1);
            yield return new WaitForSeconds(1f);

            // Assert
            Assert.AreEqual(player.Health.Max - 1, player.Health.Current);

            // Clean
            playerObject.SetActive(false);
            obstacleGameObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator contact_with_pole_reduces_health_by_one()
        {
            //Arrange
            CreatePlayer(out GameObject playerObject, 
                out PlayerCharacter player);
            GameObject poleGameObject = CreatePole();

            // Act
            player.PlayerInput.Horizontal.Returns(1);
            yield return new WaitForSeconds(1f);

            // Assert
            Assert.AreEqual(2, player.Health.Current);

            // Clean
            playerObject.SetActive(false);
            poleGameObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator contact_with_waste_grants_debuff()
        {
            //Arrange
            CreatePlayer(out GameObject playerObject, 
                out PlayerCharacter player);
            GameObject wasteGameObject = CreateWaste();

            // Act
            player.PlayerInput.Horizontal.Returns(1);
            yield return new WaitForSeconds(1f);

            // Assert
            Assert.IsTrue(player.CollisionHandler.IsDebuffed);

            // Clean
            playerObject.SetActive(false);
            wasteGameObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator debuff_wears_off_after_three_seconds()
        {
            //Arrange
            CreatePlayer(out GameObject playerObject, 
                out PlayerCharacter player);
            GameObject wasteGameObject = CreateWaste();

            // Act
            player.PlayerInput.Horizontal.Returns(1);
            while (!player.CollisionHandler.IsDebuffed)
            {
                yield return null;
            }
            yield return new WaitForSeconds(3.0f);

            // Assert
            Assert.IsFalse(player.CollisionHandler.IsDebuffed);

            // Clean
            playerObject.SetActive(false);
            wasteGameObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator debuffed_with_negative_horizontal_input_moves_right()
        {
            //Arrange
            CreatePlayer(out GameObject playerObject, 
                out PlayerCharacter player);

            float initY = player.transform.position.y;
            float initZ = player.transform.position.z;

            //Act
            player.PlayerInput.Horizontal.Returns(-1);
            player.CollisionHandler.IsDebuffed = true;
            yield return new WaitForSeconds(0.3f);

            // Assert
            Assert.IsTrue(player.transform.position.x > 0);
            Assert.AreEqual(initY, player.transform.position.y);
            Assert.AreEqual(initZ, player.transform.position.z);

            // Clean
            playerObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator debuffed_with_positive_horizontal_input_moves_left()
        {
            //Arrange
            CreatePlayer(out GameObject playerObject, 
                out PlayerCharacter player);

            float initY = player.transform.position.y;
            float initZ = player.transform.position.z;

            //Act
            player.PlayerInput.Horizontal.Returns(1);
            player.CollisionHandler.IsDebuffed = true;
            yield return new WaitForSeconds(0.3f);

            // Assert
            Assert.IsTrue(player.transform.position.x < 0);
            Assert.AreEqual(initY, player.transform.position.y);
            Assert.AreEqual(initZ, player.transform.position.z);

            // Clean
            playerObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator drags_dog_left()
        {
            // Arrange
            CreatePlayer(out GameObject playerObject,
                out PlayerCharacter player);
            CreateDog(out GameObject dogObject, out DogCharacter dog);
            CreateLeash(out GameObject leashObject, 
                out Leash leash, player, dog);

            player.Dog = dog;
            player.Leash = leash;
            dog.Player = player;
            player.Leash.LeashLength = 0.5f;

            // Act
            player.PlayerInput.Horizontal.Returns(-1);
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.IsTrue(dogObject.transform.position.x < 0.0f);

            // Clean
            playerObject.SetActive(false);
            dogObject.SetActive(false);
            leashObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator drags_dog_right()
        {
            // Arrange
            CreatePlayer(out GameObject playerObject, 
                out PlayerCharacter player);
            CreateDog(out GameObject dogObject, out DogCharacter dog);
            CreateLeash(out GameObject leashObject, 
                out Leash leash, player, dog);

            player.Dog = dog;
            player.Leash = leash;
            dog.Player = player;
            player.Leash.LeashLength = 0.5f;

            // Act
            player.PlayerInput.Horizontal.Returns(1);
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.IsTrue(dogObject.transform.position.x > 0.0f);

            // Clean
            playerObject.SetActive(false);
            dogObject.SetActive(false);
            leashObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator left_clicking_and_holding_creates_tension()
        {
            // Arrange
            CreatePlayer(out GameObject playerObject,
                out PlayerCharacter player);
            CreateDog(out GameObject dogObject, out DogCharacter dog);
            CreateLeash(out GameObject leashObject, 
                out Leash leash, player, dog);

            player.Dog = dog;
            player.Leash = leash;
            dog.Player = player;

            // Act
            player.PlayerInput.MouseLeftClicked.Returns(true);
            yield return new WaitForSeconds(0.1f);

            // Assert
            Assert.IsTrue(player.Leash.InTension);

            // Clean
            playerObject.SetActive(false);
            dogObject.SetActive(false);
            leashObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator obeys_leash_length_when_dragging_left()
        {
            // Arrange
            CreateCamera(out GameObject cameraObject);
            CreatePlayer(out GameObject playerObject,
                out PlayerCharacter player);
            CreateDog(out GameObject dogObject, out DogCharacter dog,
                new Vector3(1.0f, 0.0f, 2.0f));
            CreateLeash(out GameObject leashObject,
                out Leash leash, player, dog);

            player.Dog = dog;
            player.Leash = leash;
            dog.Player = player;

            // Act
            Vector3 playerScreenPos =
                Camera.main.WorldToScreenPoint(playerObject.transform.position);
            player.PlayerInput.MousePosition.Returns(playerScreenPos);
            player.PlayerInput.MouseLeftClicked.Returns(true);
            yield return null;
            player.PlayerInput.MousePosition.Returns(playerScreenPos -
                new Vector3(5.0f, 0.0f, 0.0f));
            yield return new WaitForSeconds(2.0f);
            player.PlayerInput.MouseLeftClicked.Returns(false);

            // Assert
            Assert.AreEqual(playerObject.transform.position.x -
                player.Leash.LeashLength, player.dragPos.x);

            // Clean
            cameraObject.SetActive(false);
            playerObject.SetActive(false);
            dogObject.SetActive(false);
            leashObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator obeys_leash_length_when_dragging_right()
        {
            // Arrange
            CreateCamera(out GameObject cameraObject);
            CreatePlayer(out GameObject playerObject,
                out PlayerCharacter player);
            CreateDog(out GameObject dogObject, out DogCharacter dog,
                new Vector3(1.0f, 0.0f, 2.0f));
            CreateLeash(out GameObject leashObject, 
                out Leash leash, player, dog);

            player.Dog = dog;
            player.Leash = leash;
            dog.Player = player;

            // Act
            Vector3 playerScreenPos = 
                Camera.main.WorldToScreenPoint(playerObject.transform.position);
            player.PlayerInput.MousePosition.Returns(playerScreenPos);
            player.PlayerInput.MouseLeftClicked.Returns(true);
            yield return null;
            player.PlayerInput.MousePosition.Returns(playerScreenPos + 
                new Vector3(5.0f, 0.0f, 0.0f));
            yield return new WaitForSeconds(2.0f);
            player.PlayerInput.MouseLeftClicked.Returns(false);

            // Assert
            Assert.AreEqual(playerObject.transform.position.x +
                player.Leash.LeashLength, player.dragPos.x);

            // Clean
            cameraObject.SetActive(false);
            playerObject.SetActive(false);
            dogObject.SetActive(false);
            leashObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator obeys_max_x_pos_constraint()
        {
            // Arrange
            CreatePlayer(out GameObject playerObject, 
                out PlayerCharacter player, new Vector3(2.9f, 0.0f, 0.0f));


            // Act
            player.PlayerInput.Horizontal.Returns(1);
            yield return new WaitForSeconds(0.5f);

            // Assert
            Assert.AreEqual(Obstacle.BoundXMax, 
                playerObject.transform.position.x, 0.1f);

            // Clean
            playerObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator obeys_min_x_pos_constraint()
        {
            // Arrange
            CreatePlayer(out GameObject playerObject, 
                out PlayerCharacter player, new Vector3(-2.9f, 0.0f, 0.0f));

            // Act
            player.PlayerInput.Horizontal.Returns(-1);
            yield return new WaitForSeconds(0.5f);

            // Assert
            Assert.AreEqual(Obstacle.BoundXMin, 
                playerObject.transform.position.x, 0.1f);

            // Clean
            playerObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator obeys_negative_max_velocity_constraint()
        {
            // Arrange
            CreatePlayer(out GameObject playerObject,
                out PlayerCharacter player);
            playerObject.transform.position = new Vector3(3.0f, 0.0f, 0.0f);

            // Act
            player.PlayerInput.Horizontal.Returns(-1);
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.AreEqual(-player.MaxVel, player.Rb.velocity.x, 0.1f);

            // Clean
            playerObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator obeys_positive_max_velocity_constraint()
        {
            // Arrange
            CreatePlayer(out GameObject playerObject, 
                out PlayerCharacter player);
            playerObject.transform.position = new Vector3(-3.0f, 0.0f, 0.0f);

            // Act
            player.PlayerInput.Horizontal.Returns(1);
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.AreEqual(player.MaxVel, player.Rb.velocity.x, 0.1f);

            // Clean
            playerObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator picks_up_coin_object()
        {
            // Arrange
            CreatePlayer(out GameObject playerObject,
                out PlayerCharacter player);
            player.Bank.CoinsCollected = 0;

            GameObject coinGameObject = new GameObject("Coin");
            coinGameObject.transform.tag = "Coin";
            coinGameObject.AddComponent<Obstacle>();
            coinGameObject.transform.position = Vector3.right;

            // Act
            player.PlayerInput.Horizontal.Returns(1);
            yield return new WaitForSeconds(0.5f);

            // Assert
            Assert.AreEqual(1, player.Bank.CoinsCollected);

            // Clean
            playerObject.SetActive(false);
            coinGameObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator stops_with_no_horizontal_input()
        {
            // Arrange
            CreatePlayer(out GameObject playerObject,
                out PlayerCharacter player);

            // Act
            player.PlayerInput.Horizontal.Returns(1);
            yield return new WaitForSeconds(0.3f);
            player.PlayerInput.Horizontal.Returns(0);
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.AreEqual(0.0f, player.Rb.velocity.x, 0.1f);

            // Clean
            playerObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator with_negative_horizontal_input_moves_left()
        {
            // Arrange
            CreatePlayer(out GameObject playerObject, 
                out PlayerCharacter player);

            float initY = player.transform.position.y;
            float initZ = player.transform.position.z;

            // Act
            player.PlayerInput.Horizontal.Returns(-1);
            yield return new WaitForSeconds(0.3f);

            // Assert
            Assert.IsTrue(player.transform.position.x < 0);
            Assert.AreEqual(initY, player.transform.position.y);
            Assert.AreEqual(initZ, player.transform.position.z);

            // Clean
            playerObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator with_positive_horizontal_input_moves_right()
        {
            // Arrange
            CreatePlayer(out GameObject playerObject, 
                out PlayerCharacter player);

            float initY = player.transform.position.y;
            float initZ = player.transform.position.z;

            // Act
            player.PlayerInput.Horizontal.Returns(1);
            yield return new WaitForSeconds(0.3f);

            // Assert
            Assert.IsTrue(player.transform.position.x > 0);
            Assert.AreEqual(initY, player.transform.position.y);
            Assert.AreEqual(initZ, player.transform.position.z);

            // Clean
            playerObject.SetActive(false);
        }

        private void CreateCamera(out GameObject cameraObject)
        {
            cameraObject = new GameObject("Main Camera");
            cameraObject.AddComponent<Camera>();
            cameraObject.transform.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0, 10, 0);
            cameraObject.transform.eulerAngles = 
                new Vector3(67.72f, -0.103f, 0);
        }

        private void CreateDog(out GameObject dogObject, out DogCharacter dog)
        {
            dogObject = new GameObject("Dog");
            dogObject.transform.position = new Vector3(0, 0, 2);
            dog = dogObject.AddComponent<DogCharacter>();
        }

        private void CreateDog(out GameObject dogObject,
            out DogCharacter dog, Vector3 spawnPosition)
        {
            dogObject = new GameObject("Dog");
            dogObject.transform.position = spawnPosition;
            dog = dogObject.AddComponent<DogCharacter>();
        }

        private void CreateLeash(out GameObject leashObject,
            out Leash leash, PlayerCharacter player, DogCharacter dog)
        {
            leashObject = new GameObject("Leash");
            leash = leashObject.AddComponent<Leash>();
            leash.Dog = dog;
            leash.Player = player;
        }

        private GameObject CreateObstacle()
        {
            GameObject obstacleGameObject = new GameObject("Obstacle");
            obstacleGameObject.transform.tag = "Obstacle";
            obstacleGameObject.transform.position = new Vector3(2, 0, 0);
            obstacleGameObject.AddComponent<Obstacle>();
            Rigidbody rb = obstacleGameObject.GetComponent<Rigidbody>();
            rb.useGravity = false;
            return obstacleGameObject;
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
        private void CreatePlayer(out GameObject playerObject,
            out PlayerCharacter player, Vector3 spawnPosition)
        {
            playerObject = new GameObject("Player");
            playerObject.transform.position = spawnPosition;
            player = playerObject.AddComponent<PlayerCharacter>();
            player.CollisionHandler =
                playerObject.AddComponent<PlayerCollisionHandler>();
            player.PlayerInput = Substitute.For<IPlayerInput>();
        }
        private GameObject CreatePole()
        {
            GameObject poleGameObject = new GameObject("Pole");
            poleGameObject.transform.tag = "Pole";
            poleGameObject.transform.position = new Vector3(2, 0, 0);
            poleGameObject.AddComponent<Obstacle>();
            Rigidbody rb = poleGameObject.GetComponent<Rigidbody>();
            rb.useGravity = false;
            return poleGameObject;
        }
        private GameObject CreateWaste()
        {
            GameObject wasteGameObject = new GameObject("Waste");
            wasteGameObject.transform.tag = "Waste";
            wasteGameObject.transform.position = new Vector3(2, 0, 0);
            wasteGameObject.AddComponent<Obstacle>();
            Rigidbody rb = wasteGameObject.GetComponent<Rigidbody>();
            rb.useGravity = false;
            return wasteGameObject;
        }
    }
}