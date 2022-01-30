using Dogger.Spawning;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class obstacle
    {
        [UnityTest]
        public IEnumerator inactivates_when_behind_the_despawn_line()
        {
            // Arrange
            GameObject obstacleObject = new GameObject("Obstacle");
            obstacleObject.transform.position = new Vector3(0.0f, 0.0f, 2.0f);
            Obstacle obstacle = obstacleObject.AddComponent<Obstacle>();
            obstacle.ScrollerRb = obstacleObject.GetComponent<Rigidbody>();
            obstacle.ScrollerRb.velocity = Vector3.back * 8.0f;

            // Act
            yield return new WaitForSeconds(0.6f);

            // Assert
            Assert.IsTrue(!obstacleObject.activeInHierarchy);

            // Clean
            obstacleObject.SetActive(false);
        }
    }
}

