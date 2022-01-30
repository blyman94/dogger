using Dogger.Core;
using RopeMinikit;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dogger.Dog
{
	/// <summary>
	/// Handles 3D graphics and animation for dog objects.
	/// </summary>
	[RequireComponent(typeof(Animator))]
	public class DogGraphics : MonoBehaviour
	{
		public BoxCollider DogCollider { get; set; }

		[SerializeField] private SessionPreferences_SO sessionPrefs;
		[SerializeField] private RopeRigidbodyConnection DogConnection;
		[SerializeField] private GameObject[] DogPrefabs;
		[SerializeField] private Vector3[] LocalPointOnBody;
		private Animator animator;

		public void Awake()
		{
			if (sessionPrefs != null && DogPrefabs != null)
			{
				GameObject dogGFX = Instantiate(DogPrefabs[sessionPrefs.DogID],
					transform);
				DogCollider = dogGFX.GetComponent<BoxCollider>();
				DogCollider.isTrigger = true;

				animator = dogGFX.GetComponent<Animator>();

				DogConnection.localPointOnBody = 
					LocalPointOnBody[sessionPrefs.DogID];
			}
		}

		public void Start()
		{
			SetDogSpeed();
		}

		/// <summary>
		/// Sets the animation speed of the dog based on player difficulty.
		/// Animation will play faster at higher difficulties.
		/// </summary>
		public void SetDogSpeed()
		{
			if (animator != null)
			{
				// Constant speed (walk) for tutorial scene.
				if (SceneManager.GetActiveScene().name == "Tutorial")
				{
					animator.SetFloat("Speed_f", 0.7f);
				}
				else
				{
					switch (sessionPrefs.Difficulty)
					{
						case 0:
							animator.SetFloat("Speed_f", 0.7f);
							break;
						case 1:
							animator.SetFloat("Speed_f", 0.8f);
							break;
						case 2:
							animator.SetFloat("Speed_f", 0.9f);
							break;
					}
				}
			}
		}
	}
}