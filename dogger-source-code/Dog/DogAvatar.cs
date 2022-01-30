using System.Collections;
using UnityEngine;

namespace Dogger.Dog
{
	/// <summary>
	/// Visual representation of a dog object during the dog selection process.
	/// </summary>
	[RequireComponent(typeof(Animator))]
	public class DogAvatar : MonoBehaviour
	{
		private Animator animator;
		private bool isIdle;

		private void Awake()
		{
			animator = GetComponent<Animator>();
		}

		private void OnEnable()
		{
			StartCoroutine(RandomAnimationRoutine());
		}

		private void Update()
		{
			transform.Rotate(Vector3.up * -30.0f * Time.deltaTime);
		}

		public void IdleStart()
		{
			isIdle = true;
		}

		/// <summary>
		/// Randomly chooses one of the dogs animations to showcase behaviour.
		/// </summary>
		/// <returns> Waits for a few seconds in between animations.</returns>
		private IEnumerator RandomAnimationRoutine()
		{
			yield return new WaitForSeconds(3.0f);
			int randomAnim = Random.Range(0, 3);
			animator.SetTrigger("trigger" + randomAnim);
			isIdle = false;
			while (!isIdle)
			{
				yield return null;
			}
			StartCoroutine(RandomAnimationRoutine());
		}
	}
}

