using Dogger.Dog;
using UnityEngine;

namespace Dogger.Player
{
    /// <summary>
    /// An object representing the unbreakable connecting between player and 
    /// dog.
    /// </summary>
    public class Leash : MonoBehaviour
    {
        /// <summary>
        /// Dog the leash is attached to.
        /// </summary>
        public DogCharacter Dog;

        /// <summary>
        /// Tracks whether the player and dog's leash is in a state of tension.
        /// </summary>
        public bool InTension;

        /// <summary>
        /// Maximum distance between player and dog before leash transitions 
        /// into tension.
        /// </summary>
        public float LeashLength = 1.0f;

        /// <summary>
        /// Player the leash is attached to.
        /// </summary>
        public PlayerCharacter Player;

        /// <summary>
        /// Transform of the dog the leash is attached to.
        /// </summary>
        private Transform dogTransform;

        /// <summary>
        /// Transform of the player the leash is attached to.
        /// </summary>
        private Transform playerTransform;

        /// <summary>
        /// Current direction of the dog relative to the player.
        /// </summary>
        public float DogDir { get; set; }

        private void Start()
        {
            playerTransform = Player.gameObject.transform;
            dogTransform = Dog.gameObject.transform;
        }

        private void Update()
        {
            CheckTension();
            UpdateDogDirection();
        }

        /// <summary>
        /// Determines whether there is tension on the leash connecting the 
        /// dog and player. The player and dog behave differently when their 
        /// leash is in tension.
        /// </summary>
        private void CheckTension()
        {
            if (Dog != null)
            {
                if (Player.PlayerInput.MouseLeftClicked)
                {
                    InTension = true;
                }
                else
                {
                    if (Mathf.Abs(dogTransform.position.x - playerTransform.position.x) > LeashLength)
                    {
                        InTension = true;
                    }
                    else
                    {
                        InTension = false;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the direction of the dog in relation to the player.
        /// </summary>
        private void UpdateDogDirection()
        {
            if (Dog != null)
            {
                DogDir = Mathf.Sign(dogTransform.position.x - playerTransform.position.x);
            }
        }
    }
}