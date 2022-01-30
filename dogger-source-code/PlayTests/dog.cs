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
    public class dog
    {
        [UnityTest]
        public IEnumerator avoids_obstacle()
        {
            // Arrange
            CreateDog(out GameObject dogObject, out DogCharacter dog);

            GameObject obstacleObject = 
                CreateObstacle(new Vector3(0.0f, 0.0f, 4.0f));
            obstacleObject.GetComponent<Rigidbody>().velocity = 
                Vector3.back * 2.0f;

            // Act
            dog.AddObjectToFocusQueue(obstacleObject);
            yield return new WaitForSeconds(2.0f);

            // Assert
            Assert.AreEqual(0, dog.ObstaclesHit);

            // Clean
            dogObject.SetActive(false);
            obstacleObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator avoids_obstacle_and_picks_up_waste_when_obstacle_is_first()
        {
            // Arrange
            CreateDog(out GameObject dogObject, out DogCharacter dog,
                new Vector3(0.0f, 0.0f, 0.0f));

            GameObject obstacleObject = 
                CreateObstacle(new Vector3(0.0f, 0.0f, 4.0f));
            obstacleObject.GetComponent<Rigidbody>().velocity = 
                Vector3.back * 2.0f;

            GameObject wasteObject = 
                CreateWaste(new Vector3(0.0f, 0.0f, 8.0f));
            wasteObject.GetComponent<Rigidbody>().velocity = 
                Vector3.back * 2.0f;

            // Act
            dog.AddObjectToFocusQueue(obstacleObject);
            dog.AddObjectToFocusQueue(wasteObject);
            yield return new WaitForSeconds(4.5f);

            // Assert
            Assert.AreEqual(0, dog.ObstaclesHit);
            Assert.AreEqual(1, dog.WasteCollected);

            // Clean
            dogObject.SetActive(false);
            obstacleObject.SetActive(false);
            wasteObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator avoids_obstacle_and_picks_up_waste_when_waste_is_first()
        {
            // Arrange
            CreateDog(out GameObject dogObject, out DogCharacter dog,
                new Vector3(4.0f, 0.0f, 0.0f));

            GameObject wasteObject = 
                CreateWaste(new Vector3(2.0f, 0.0f, 4.0f));
            wasteObject.GetComponent<Rigidbody>().velocity = 
                Vector3.back * 2.0f;

            GameObject obstacleObject = 
                CreateObstacle(new Vector3(2.0f, 0.0f, 8.0f));
            obstacleObject.GetComponent<Rigidbody>().velocity = 
                Vector3.back * 2.0f;

            // Act
            dog.AddObjectToFocusQueue(wasteObject);
            dog.AddObjectToFocusQueue(obstacleObject);
            yield return new WaitForSeconds(4.5f);

            // Assert
            Assert.AreEqual(0, dog.ObstaclesHit);
            Assert.AreEqual(1, dog.WasteCollected);

            // Clean
            dogObject.SetActive(false);
            obstacleObject.SetActive(false);
            wasteObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator avoids_obstacle_by_moving_left_if_obstacle_is_too_far_right()
        {
            // Arrange
            Vector3 dogStartPos = new Vector3(4.0f, 0.0f, 0.0f);
            CreateDog(out GameObject dogObject, out DogCharacter dog,
                dogStartPos);

            GameObject obstacleObject = 
                CreateObstacle(new Vector3(4.0f, 0.0f, 4.0f));
            obstacleObject.GetComponent<Rigidbody>().velocity = 
                Vector3.back * 2.0f;

            // Act
            dog.AddObjectToFocusQueue(obstacleObject);
            yield return new WaitForSeconds(2.0f);

            // Assert
            Assert.AreEqual(0, dog.ObstaclesHit);
            Assert.IsTrue(dogObject.transform.position.x < dogStartPos.x);

            // Clean
            dogObject.SetActive(false);
            obstacleObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator avoids_obstacle_by_moving_right_if_obstacle_is_too_far_left()
        {
            // Arrange
            Vector3 dogStartPos = new Vector3(-4.0f, 0.0f, 0.0f);
            CreateDog(out GameObject dogObject, out DogCharacter dog,
                dogStartPos);

            GameObject obstacleObject = 
                CreateObstacle(new Vector3(-4.0f, 0.0f, 4.0f));
            obstacleObject.GetComponent<Rigidbody>().velocity = 
                Vector3.back * 2.0f;

            // Act
            dog.AddObjectToFocusQueue(obstacleObject);
            yield return new WaitForSeconds(2.0f);

            // Assert
            Assert.AreEqual(0, dog.ObstaclesHit);
            Assert.IsTrue(dogObject.transform.position.x > dogStartPos.x);

            // Clean
            dogObject.SetActive(false);
            obstacleObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator drags_player_left()
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
            dog.TargetPos = new Vector3(-6.0f, 0.0f, 0.0f);
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.IsTrue(playerObject.transform.position.x < 0.0f);

            // Clean
            playerObject.SetActive(false);
            dogObject.SetActive(false);
            leashObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator drags_player_right()
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
            dog.TargetPos = new Vector3(6.0f, 0.0f, 0.0f);
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.IsTrue(playerObject.transform.position.x > 0.0f);

            // Clean
            playerObject.SetActive(false);
            dogObject.SetActive(false);
            leashObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator ignores_waste_object_to_left_if_not_close_enough()
        {
            // Arrange
            CreateDog(out GameObject dogObject, out DogCharacter dog,
                new Vector3(3.0f, 0.0f, 0.0f));

            GameObject wasteObject = 
                CreateWaste(new Vector3(-3.0f, 0.0f, 2.0f));
            wasteObject.GetComponent<Rigidbody>().AddForce(Vector3.back * 4,
                ForceMode.VelocityChange);

            // Act
            yield return new WaitForSeconds(0.3f);
            dog.AddObjectToFocusQueue(wasteObject);
            yield return new WaitForSeconds(0.3f);

            // Assert
            Assert.AreEqual(new Vector3(3.0f, 0.0f, 0.0f),
                dogObject.transform.position);

            // Clean
            dogObject.SetActive(false);
            wasteObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator ignores_waste_object_to_right_if_not_close_enough()
        {
            // Arrange
            CreateDog(out GameObject dogObject, out DogCharacter dog,
                new Vector3(-3.0f, 0.0f, 0.0f));

            GameObject wasteObject = 
                CreateWaste(new Vector3(3.0f, 0.0f, 3.0f));
            wasteObject.GetComponent<Rigidbody>().AddForce(Vector3.back * 4,
                ForceMode.VelocityChange);

            // Act
            yield return new WaitForSeconds(0.3f);
            dog.AddObjectToFocusQueue(wasteObject);
            yield return new WaitForSeconds(0.3f);

            // Assert
            Assert.AreEqual(new Vector3(-3.0f, 0.0f, 0.0f),
                dogObject.transform.position);

            // Clean
            dogObject.SetActive(false);
            wasteObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator obeys_max_x_pos_constraint()
        {
            // Arrange
            CreateDog(out GameObject dogObject, out DogCharacter dog,
                new Vector3(4.8f, 0.0f, 0.0f));
            Rigidbody dogRb = dogObject.GetComponent<Rigidbody>();

            // Act
            dog.TargetPos = new Vector3(6, 0, 0);
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.AreEqual(Obstacle.BoundXMax, 
                dogRb.transform.position.x, 0.1f);

            // Clean
            dogObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator obeys_min_x_pos_constraint()
        {
            // Arrange
            CreateDog(out GameObject dogObject, out DogCharacter dog,
                new Vector3(-4.8f, 0.0f, 0.0f));
            Rigidbody dogRb = dogObject.GetComponent<Rigidbody>();

            // Act
            dog.TargetPos = new Vector3(-6, 0, 0);
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.AreEqual(Obstacle.BoundXMin, 
                dogRb.transform.position.x, 0.1f);

            // Clean
            dogObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator obeys_negative_max_velocity_constraint()
        {
            // Arrange
            CreateDog(out GameObject dogObject, out DogCharacter dog,
                new Vector3(4.8f, 0.0f, 0.0f));

            // Act
            dog.TargetPos = new Vector3(-6.0f, 0.0f, 0.0f);
            yield return new WaitForSeconds(1f);

            // Assert
            Assert.AreEqual(-dog.MaxVel, dog.Rb.velocity.x, 0.1f);

            // Clean
            dogObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator obeys_positive_max_velocity_constraint()
        {
            // Arrange
            CreateDog(out GameObject dogObject, out DogCharacter dog, 
                new Vector3(-4.8f, 0.0f, 0.0f));

            // Act
            dog.TargetPos = new Vector3(6.0f, 0.0f, 0.0f);
            yield return new WaitForSeconds(1f);

            // Assert
            Assert.AreEqual(dog.MaxVel, dog.Rb.velocity.x, 0.1f);

            // Clean
            dogObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator picking_up_waste_reduces_player_health()
        {
            // Arrange
            CreateDog(out GameObject dogObject, out DogCharacter dog,
                new Vector3(0.0f,0.0f,2.0f));
            CreatePlayer(out GameObject playerObject, 
                out PlayerCharacter player);
            dog.Player = player;
            player.Dog = dog;

            GameObject wasteObject = CreateWaste(new Vector3(0, 0, 4));
            wasteObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, -4.0f), 
                ForceMode.VelocityChange);

            // Act
            yield return new WaitForSeconds(0.5f);

            // Assert
            Assert.AreEqual(2, player.Health.Current);

            // Clean
            dogObject.SetActive(false);
            playerObject.SetActive(false);
            wasteObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator picks_up_waste()
        {
            // Arrange
            CreateDog(out GameObject dogObject, out DogCharacter dog);
            GameObject wasteObject = CreateWaste(new Vector3(0, 0, 4));
            wasteObject.GetComponent<Rigidbody>().velocity = 
                new Vector3(0, 0, -4.0f);

            // Act
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.AreEqual(1, dog.WasteCollected);

            // Clean
            dogObject.SetActive(false);
            wasteObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator seeks_waste_object_to_left_if_close_enough()
        {
            // Arrange
            CreateDog(out GameObject dogObject, out DogCharacter dog);

            GameObject wasteObject = CreateWaste(new Vector3(-2.0f, 0.0f, 4.0f));
            wasteObject.GetComponent<Rigidbody>().velocity = Vector3.back;

            // Act
            dog.AddObjectToFocusQueue(wasteObject);
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.AreEqual(wasteObject.transform.position.x,
                dogObject.transform.position.x, dog.DogCollider.size.x);

            // Clean
            dogObject.SetActive(false);
            wasteObject.SetActive(false);
        }

        [UnityTest]
        public IEnumerator seeks_waste_object_to_right_if_close_enough()
        {
            // Arrange
            CreateDog(out GameObject dogObject, out DogCharacter dog);

            GameObject wasteObject = CreateWaste(new Vector3(-2.0f, 0.0f, 4.0f));
            wasteObject.GetComponent<Rigidbody>().velocity = Vector3.back;

            // Act
            dog.AddObjectToFocusQueue(wasteObject);
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.AreEqual(wasteObject.transform.position.x,
                dogObject.transform.position.x, dog.DogCollider.size.x);

            // Clean
            dogObject.SetActive(false);
            wasteObject.SetActive(false);
        }

        private static void CreateDog(out GameObject dogObject,
            out DogCharacter dog)
        {
            dogObject = new GameObject("Dog");
            dogObject.transform.position = Vector3.zero;
            dog = dogObject.AddComponent<DogCharacter>();
        }

        private static void CreateDog(out GameObject dogObject,
            out DogCharacter dog, Vector3 spawnPosition)
        {
            dogObject = new GameObject("Dog");
            dogObject.transform.position = spawnPosition;
            dog = dogObject.AddComponent<DogCharacter>();
        }

        private static GameObject CreateObstacle(Vector3 spawnPos)
        {
            GameObject obstacleObject = new GameObject("Obstacle");
            obstacleObject.transform.tag = "Obstacle";
            obstacleObject.layer = 9;
            obstacleObject.transform.position = spawnPos;
            obstacleObject.AddComponent<Obstacle>();
            Rigidbody rb = obstacleObject.GetComponent<Rigidbody>();
            rb.useGravity = false;
            return obstacleObject;
        }

        private static GameObject CreateWaste(Vector3 spawnPos)
        {
            GameObject wasteObject = new GameObject("Waste");
            wasteObject.transform.tag = "Waste";
            wasteObject.transform.position = spawnPos;
            wasteObject.AddComponent<Obstacle>();
            Rigidbody rb = wasteObject.GetComponent<Rigidbody>();
            rb.useGravity = false;
            return wasteObject;
        }

        private void CreateLeash(out GameObject leashObject, out Leash leash,
            PlayerCharacter player, DogCharacter dog)
        {
            leashObject = new GameObject("Leash");
            leash = leashObject.AddComponent<Leash>();
            leash.Dog = dog;
            leash.Player = player;
        }

        private void CreatePlayer(out GameObject playerObject,
            out PlayerCharacter player)
        {
            playerObject = new GameObject("Player");
            player = playerObject.AddComponent<PlayerCharacter>();
            player.PlayerInput = Substitute.For<IPlayerInput>();
        }
    }
}