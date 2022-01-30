using Dogger.Movement;
using Dogger.Player;
using Dogger.Spawning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dogger.Dog
{
	/// <summary>
	/// The dog object used in main gameplay. Behaviours include avoiding 
	/// obstacles, seeking waste, and remaining within a given leash length 
	/// of the player.
	/// </summary>
	[RequireComponent(typeof(Rigidbody))]
	public class DogCharacter : Mover
	{
		/// <summary>
		/// Constant z position of the transform.
		/// </summary>
		public static float zPos = 2.0f;

		/// <summary>
		/// 3D graphics to represent the dog in game.
		/// </summary>
		public DogGraphics DogGFX;

		/// <summary>
		/// The dog's associated player.
		/// </summary>
		public PlayerCharacter Player;

		/// <summary>
		/// A small buffer on either side of an obstacle to ensure every size 
		/// of dog does not appear to collide with an obstacle it avoids.
		/// </summary>
		private readonly float avoidObstacleBuffer = 0.1f;

		private BoxCollider currentObjectCollider;

		private bool isProcessingObject;

		/// <summary>
		/// Tracks the position of the dog's associated player.
		/// </summary>
		private Vector3 playerPos;

		/// <summary>
		/// The current object the dog is focused on. If it's an obstacle, the
		/// dog will avoid it. If it's waste, the dog will seek it.
		/// </summary>
		public GameObject CurrentObject { get; set; }

		public BoxCollider DogCollider { get; set; }

		/// <summary>
		/// The queue of objects the dog will focus on.
		/// </summary>
		public Queue<GameObject> FocusQueue { get; set; }

		/// <summary>
		/// Number of objects collided with, used in unit tests.
		/// </summary>
		public int ObstaclesHit { get; set; }

		/// <summary>
		/// Number of waste objects collected, used in unit tests.
		/// </summary>
		public int WasteCollected { get; set; }

		public delegate void PickedUpWaste(int health);
		public static PickedUpWaste pickedUpWaste;

		public delegate void ProcessObjectSignal();
		public ProcessObjectSignal processObjectSignal;

		protected override void Awake()
		{
			base.Awake();

			FocusQueue = new Queue<GameObject>();

			TargetPos = transform.position;
		}

		private void Start()
		{
			if (DogGFX != null)
			{
				DogCollider = DogGFX.DogCollider;
			}
			else
			{
				DogCollider = gameObject.AddComponent<BoxCollider>();
				DogCollider.isTrigger = true;
			}
		}

		protected override void FixedUpdate()
		{
			if (Player != null)
			{
				if (Player.PlayerInput.MouseLeftClicked)
				{
					RespondToPlayerDrag();
				}
				else if (Player.Horizontal != 0 && Player.Leash.InTension &&
					Player.Leash.DogDir != Mathf.Sign(Player.Horizontal))
				{
					// Change target position if dragged by player movement.
					TargetPos = playerPos;
				}
			}

			ConstrainPositionToObstacleBounds();

			base.FixedUpdate();
		}

		private void Update()
		{
			if (Player != null)
			{
				playerPos = Player.gameObject.transform.position;
			}
			if (CurrentObject != null)
			{
				DetectObjectPassage();
			}
		}

		private void OnEnable()
		{
			processObjectSignal += ProcessObject;
		}

		private void OnDisable()
		{
			processObjectSignal -= ProcessObject;
		}
		
		private void OnTriggerEnter(Collider other)
		{
			if (other.transform.CompareTag("Waste"))
			{
				WasteCollected += 1;
				if (Player != null)
				{
					Player.Health.Decrement(1);
				}
				other.gameObject.SetActive(false);
				isProcessingObject = false;
				pickedUpWaste?.Invoke(Player.Health.Current);
				processObjectSignal?.Invoke();
			}
			else if (other.transform.CompareTag("Obstacle") ||
				other.transform.CompareTag("Pole"))
			{
				ObstaclesHit += 1;
			}
		}

		/// <summary>
		/// Enqueues a spawned object for the dog to focus on.
		/// </summary>
		/// <param name="objectToAdd">
		/// The object for focus (i.e. waste or obstacle).
		/// </param>
		public void AddObjectToFocusQueue(GameObject objectToAdd)
		{
			FocusQueue.Enqueue(objectToAdd);
			if (!isProcessingObject)
			{
				processObjectSignal?.Invoke();
				isProcessingObject = true;
			}
		}

		/// <summary>
		/// Instructs the dog how to avoid an obstacle. Chooses a random
		/// direction to avoid towards, then moves the dog in that direction 
		/// until its collider no longer overlaps with the obstacle's collider
		/// on the x axis.
		/// </summary>
		private IEnumerator AvoidObstacleRoutine()
		{
			yield return null;
			bool isAvoidingObject = false;
			while (CurrentObject.transform.CompareTag("Obstacle") ||
				CurrentObject.transform.CompareTag("Pole"))
			{
				float oMin = currentObjectCollider.bounds.min.x;
				float oMax = currentObjectCollider.bounds.max.x;
				float dMin = DogCollider.bounds.min.x;
				float dMax = DogCollider.bounds.max.x;

				if ((dMin > oMax) || (dMax < oMin))
				{
					isAvoidingObject = false;
					TargetPos = transform.position;
				}
				else
				{
					if (!isAvoidingObject)
					{
						isAvoidingObject = true;
						SetAvoidTargetPosition(oMin, oMax);
					}
				}
				yield return null;
			}
		}

		/// <summary>
		/// Checks how long it would take for the dog to reach the waste on
		/// x axis against how long it will take for the waste to reach the dog
		/// on the z axis.
		/// </summary>
		/// <returns>Returns true if the dog can reach the waste on the
		/// x axis before the waste passes it on the z axis.</returns>
		private bool CheckToWasteTravelTime()
		{
			float wasteZDistanceFromDog =
				Mathf.Abs(currentObjectCollider.bounds.min.z -
				DogCollider.bounds.max.z);

			float wasteZTimeToReachDog = wasteZDistanceFromDog /
				Mathf.Abs(CurrentObject.gameObject.GetComponent<Rigidbody>().velocity.z);

			float wasteXDistanceFromDog =
				Mathf.Abs(CurrentObject.transform.position.x -
				transform.position.x);

			float dogXTimeToReachWaste = wasteXDistanceFromDog / MaxVel;

			if (dogXTimeToReachWaste < wasteZTimeToReachDog)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Ensures the dog cannot move beyond the obstacle spawn bounds.
		/// </summary>
		private void ConstrainPositionToObstacleBounds()
		{
			if (TargetPos.x > Obstacle.BoundXMax)
			{
				TargetPos.x = Obstacle.BoundXMax;
			}
			if (TargetPos.x < Obstacle.BoundXMin)
			{
				TargetPos.x = Obstacle.BoundXMin;
			}
		}

		/// <summary>
		/// Detect whether the focused object has moved behind the dog. If so,
		/// process the next object in the focus queue.
		/// </summary>
		private void DetectObjectPassage()
		{
			if (CurrentObject.transform.position.z +
								(currentObjectCollider.size.z * 0.5f) <
								transform.position.z - (DogCollider.bounds.size.z * 0.5f))
			{
				isProcessingObject = false;
				processObjectSignal?.Invoke();
			}
		}

		/// <summary>
		/// Dequeues the next object in the dog's focus queue, then determines
		/// whether the dog should avoid or seek the object based on type.
		/// </summary>
		private void ProcessObject()
		{
			isProcessingObject = true;

			// Break out of process object loop if the focus queue is empty.
			if (FocusQueue.Count == 0)
			{
				isProcessingObject = false;
				return;
			}

			CurrentObject = FocusQueue.Dequeue();
			currentObjectCollider = CurrentObject.GetComponent<BoxCollider>();

			if (CurrentObject.transform.CompareTag("Obstacle") ||
			CurrentObject.transform.CompareTag("Pole"))
			{
				StartCoroutine(AvoidObstacleRoutine());
			}
			else if (CurrentObject.transform.CompareTag("Waste"))
			{
				StartCoroutine(SeekWasteRoutine());
			}
		}

		/// <summary>
		/// Dictates the dog's behavior when the player clicks and drags with
		/// the mouse. The player's drag routine sets a new position based on 
		/// how the mouse moves once clicked and held. This method sets the
		/// dogs target position to the position dictated by that routine.
		/// </summary>
		private void RespondToPlayerDrag()
		{
			if (Player.Horizontal != 0 && Player.Leash.DogDir !=
									Mathf.Sign(Player.Horizontal))
			{
				TargetPos = playerPos;
			}
			else
			{
				TargetPos = Player.dragPos;
			}
		}

		/// <summary>
		/// Instructs the dog how to seek a waste object.
		/// </summary>
		private IEnumerator SeekWasteRoutine()
		{
			yield return null;
			while (CurrentObject.transform.CompareTag("Waste"))
			{
				bool CanReachWaste = CheckToWasteTravelTime();

				// If dog can get to waste in time, move to the waste.
				if (CanReachWaste)
				{
					TargetPos = CurrentObject.transform.position;
				}
				yield return null;
			}

		}

		/// <summary>
		/// Sets a new target position for the dog which will avoid the
		/// focused obstacle object.
		/// </summary>
		/// <param name="obstacleMinX">
		/// The minimum x position of the obstacle's collider.
		/// </param>
		/// <param name="obstacleMaxX">
		/// The maximum x position of the obstacle's collider.
		/// </param>
		private void SetAvoidTargetPosition(float obstacleMinX,
			float obstacleMaxX)
		{
			bool avoidLeft;
			float width = DogCollider.bounds.size.x;

			float leftAvoidPosX = obstacleMinX - (width * 0.5f) - avoidObstacleBuffer;
			float rightAvoidPosX = obstacleMaxX + (width * 0.5f) + avoidObstacleBuffer;

			if (rightAvoidPosX >= Obstacle.BoundXMax)
			{
				avoidLeft = true;
			}
			else if (leftAvoidPosX <= Obstacle.BoundXMin)
			{
				avoidLeft = false;
			}
			else
			{
				avoidLeft = (Random.value > 0.5f);
			}
			TargetPos = avoidLeft ? Vector3.right * leftAvoidPosX : Vector3.right * rightAvoidPosX;
		}
	}
}