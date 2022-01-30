using UnityEngine;

namespace Dogger.Core
{
	public class DontDestroyOnLoad : MonoBehaviour
	{
		public void Awake()
		{
			DontDestroyOnLoad(this);
		}
	}

}