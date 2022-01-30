using Dogger.Control;
using Dogger.Core;
using Dogger.Dog;
using Dogger.Player;
using Dogger.Spawning;
using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using RopeMinikit;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Mathematics;

namespace Tests
{
    public class rope
    {
        [UnityTest]
        public IEnumerator reduces_player_health_to_zero_on_contact_with_pole()
        {
            // Arrange
            List<GameObject> cleanupList = new List<GameObject>();

            GameObject gameManagerObject = new GameObject("gameManager");
            GameManager gameManager = 
                gameManagerObject.AddComponent<GameManager>();
            gameManager.Testing = true;
            cleanupList.Add(gameManagerObject);

            CreatePlayer(out GameObject playerObject, 
                out PlayerCharacter player, new Vector3(-1.0f, 0.0f, 0.0f));
            cleanupList.Add(playerObject);

            GameObject poleObject = new GameObject("Pole");
            poleObject.transform.tag = "Pole";
            poleObject.transform.position = new Vector3(0, 0, 1);
            Obstacle obstacle = poleObject.AddComponent<Obstacle>();
            obstacle.ScrollerRb = poleObject.GetComponent<Rigidbody>();
            obstacle.ScrollerRb.useGravity = false;
            cleanupList.Add(poleObject);

            CreateDog(out GameObject dogObject, 
                out DogCharacter dog, new Vector3(1.0f, 0.0f, 1.0f));
            cleanupList.Add(dogObject);

            player.Dog = dog;

            List<float3> spawnPoints = new List<float3>
            {
                new float3(-1, 0, 0),
                new float3(1, 0, 0)
            };

            GameObject ropeObject = new GameObject("Rope");
            cleanupList.Add(ropeObject);
            Rope rope = ropeObject.AddComponent<Rope>();
            rope.spawnPoints = spawnPoints;
            rope.simulation.enabled = true;
            rope.collisions.enabled = true;
            rope.simulation.lengthMultiplier = 0.1f;
            rope.simulation.gravityMultiplier = 0.0f;

            RopeRigidbodyConnection ropeRBC0 = 
                ropeObject.AddComponent<RopeRigidbodyConnection>();
            ropeRBC0.rope = rope;
            ropeRBC0.ropeLocation = 0;
            ropeRBC0.automaticallyFindRopeLocation = false;
            ropeRBC0.rigidbody = dog.Rb;
            ropeRBC0.localPointOnBody = Vector3.zero;
            ropeRBC0.rigidbodyDamping = 0.1f;
            ropeRBC0.stiffness = 1.0f;

            RopeRigidbodyConnection ropeRBC1 = 
                ropeObject.AddComponent<RopeRigidbodyConnection>();
            ropeRBC1.rope = rope;
            ropeRBC1.ropeLocation = 0;
            ropeRBC1.automaticallyFindRopeLocation = false;
            ropeRBC1.rigidbody = player.GetComponent<Rigidbody>();
            ropeRBC1.localPointOnBody = Vector3.zero;
            ropeRBC1.rigidbodyDamping = 0.1f;
            ropeRBC1.stiffness = 1.0f;

            // Act
            poleObject.GetComponent<Rigidbody>().AddForce(Vector3.back, 
                ForceMode.VelocityChange);
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.AreEqual(0,player.Health.Current);

            // Clean
            foreach (GameObject item in cleanupList)
            {
                item.SetActive(false);
            }
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

        private void CreateDog(out GameObject dogObject, 
            out DogCharacter dog, Vector3 spawnPosition)
        {
            dogObject = new GameObject("Dog");
            dogObject.transform.position = spawnPosition;
            dog = dogObject.AddComponent<DogCharacter>();
        }
    }
}
