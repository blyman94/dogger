using Dogger.Core;
using RopeMinikit;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dogger.Player
{
	/// <summary>
	/// Handles all graphics and animations for the player.
	/// </summary>
	public class PlayerGraphics : MonoBehaviour
	{
		/// <summary>
		/// The associated player.
		/// </summary>
		public PlayerCharacter player;

		/// <summary>
		/// The session preferences for the current session.
		/// </summary>
		[SerializeField] private SessionPreferences_SO sessionPrefs;

		/// <summary>
		/// The rigidbody connection used to connect the leash to the player's
		/// hand.
		/// </summary>
		[SerializeField] private RopeRigidbodyConnection HandConnection;

		/// <summary>
		/// An array of two prefabs representing player characters (male 
		/// and female peasents) from which a player character is randomly 
		/// chosen each session.
		/// </summary>
		[SerializeField] private GameObject[] PeasentPrefabs;

		private Animator animator;

		/// <summary>
		/// Current movement direction of the player. 0 signals the direction
		/// is straight forward.
		/// </summary>
		private float playerXDir = 0;

		/// <summary>
		/// A kinematic rigidbody connected to the weapon slot (hand bone) of
		/// the player graphics. Simulates the player holding the leash.
		/// </summary>
		private Rigidbody HandRb;

		private void Awake()
		{
			if (sessionPrefs != null && PeasentPrefabs != null)
			{
				int randInt = Random.Range(0, 2);
				GameObject peasentGFX = Instantiate(PeasentPrefabs[randInt],
					transform);

				animator = peasentGFX.GetComponent<Animator>();
				HandRb = peasentGFX.GetComponentInChildren<Rigidbody>();
				HandConnection.rigidbody = HandRb;
			}
		}

		private void Start()
		{
			if (animator != null)
			{
				SetAnimatorVerticalFloat();
			}
		}

		private void Update()
		{
			if (player != null && animator != null)
			{
				playerXDir = player.Horizontal;
				animator.SetFloat("Horizontal_f", playerXDir);
			}
		}

		/// <summary>
		/// Sets the vertical float of the player animator component to match
		/// the difficulty. The higher the difficulty, the faster the player 
		/// animation will move.
		/// </summary>
		private void SetAnimatorVerticalFloat()
		{
			if (SceneManager.GetActiveScene().name == "Tutorial")
			{
				animator.SetFloat("Vertical_f", 1.0f);
			}
			else
			{
				switch (sessionPrefs.Difficulty)
				{
					case 0:
						animator.SetFloat("Vertical_f", 1f);
						break;
					case 1:
						animator.SetFloat("Vertical_f", 1.8f);
						break;
					case 2:
						animator.SetFloat("Vertical_f", 2f);
						break;
				}
			}
		}

		
	}
}