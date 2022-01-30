using Dogger.Control;
using Dogger.Dog;
using Dogger.Movement;
using Dogger.Spawning;
using System.Collections;
using UnityEngine;

namespace Dogger.Player
{
    /// <summary>
    /// The player object used in main gameplay. Reacts to player input.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerCharacter : Mover
    {
        public Bank Bank;
        public PlayerCollisionHandler CollisionHandler;
        public DogCharacter Dog;

        /// <summary>
        /// Position to which to set the dog's TargetPosition when the player 
        /// is using click + drag to move the dog.
        /// </summary>
        public Vector3 dragPos;

        /// <summary>
        /// Rate at which dragPos changes as the player moves their mouse while
        /// using click + drag to move the dog.
        /// </summary>
        public Vector3 dragRate = new Vector3(0.5f, 0.0f, 0.0f);

        public Health Health;

        /// <summary>
        /// Horizontal input for player movement.
        /// </summary>
        public float Horizontal;

        /// <summary>
        /// Bool to check if the player is currently using click + drag to move
        /// the dog.
        /// </summary>
        public bool isDragging;

        public Leash Leash;
        public IPlayerInput PlayerInput;

        /// <summary>
        /// Current position of the player's dog.
        /// </summary>
        private Vector3 dogPos;

        protected override void Awake()
        {
            base.Awake();

            PlayerInput = new PlayerInput();

            Health = new Health(3);
            Bank = new Bank();

            TargetPos = transform.position;
        }

        private void Start()
        {
            if (Dog != null)
            {
                dogPos = Dog.gameObject.transform.position;
                dragPos = dogPos;
            }
        }

        protected override void FixedUpdate()
        {
            if (Dog != null)
            {
                if (Horizontal == 0 && Dog.TargetPos != dogPos &&
                Leash.InTension && -Leash.DogDir != Mathf.Sign(Dog.TargetPos.x - dogPos.x) &&
                !isDragging)
                {
                    TargetPos = dogPos;
                }
            }

            base.FixedUpdate();
        }

        private void Update()
        {
            GetPlayerInput();

            if (PlayerInput != null)
            {
                if (Dog != null)
                {
                    dogPos = Dog.gameObject.transform.position;
                    if (!isDragging && PlayerInput.MouseLeftClicked)
                    {
                        StartCoroutine(ClickAndDragRoutine(PlayerInput.MousePosition));
                    }
                }

                CheckAndApplyDebuff();
                MoveX();
            }
        }

        private void CheckAndApplyDebuff()
        {
            if (CollisionHandler != null)
            {
                Horizontal *= CollisionHandler.IsDebuffed ? -1 : 1;
            }
        }

        /// <summary>
        /// Reacts to the players mouse inputs (click + hold and position) by
        /// dragging the dog to a new Target position.
        /// </summary>
        /// <param name="initMousePosition">Initial mouse position.</param>
        private IEnumerator ClickAndDragRoutine(Vector3 initMousePosition)
        {
            isDragging = true;
            Vector3 currentMousePosition;
            bool firstTug = true;
            while (PlayerInput.MouseLeftClicked)
            {
                currentMousePosition = PlayerInput.MousePosition;
                float dragDir = Mathf.Sign(currentMousePosition.x - initMousePosition.x);
                if (dragDir != Leash.DogDir && currentMousePosition.x != initMousePosition.x)
                {
                    if (firstTug)
                    {
                        firstTug = false;

                        dragPos = transform.position;
                    }
                    if (Mathf.Abs(dragPos.x + dragRate.x * dragDir) < Leash.LeashLength)
                    {
                        dragPos += dragRate * dragDir;
                    }
                    else
                    {
                        dragPos = new Vector3(Leash.LeashLength * dragDir, 0.0f, 0.0f);
                    }
                }
                yield return null;
            }
            isDragging = false;
        }

        private void GetPlayerInput()
        {
            _ = PlayerInput.PauseButtonPushed;
            Horizontal = PlayerInput.Horizontal;
        }

        /// <summary>
        /// Moves the player along the x axis according to recieved Horizontal
        /// input.
        /// </summary>
        private void MoveX()
        {
            if (Horizontal > 0)
            {
                TargetPos = new Vector3(Obstacle.BoundXMax, 0.0f, 0.0f);
            }
            else if (Horizontal < 0)
            {
                TargetPos = new Vector3(Obstacle.BoundXMin, 0.0f, 0.0f);
            }
            else
            {
                TargetPos = transform.position;
            }
        }
    }
}