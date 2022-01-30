using UnityEngine;

namespace Dogger.Core
{
	/// <summary>
	/// A base class for Singletons. All extending classes will follow the Singleton pattern.
	/// </summary>
	/// <typeparam name="T">Type of singleton class (i.e. GameManager)</typeparam>
	public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
		public static T Instance { get; private set; }
		public static bool IsInitialized => Instance != null;

		protected virtual void Awake()
		{
			if (Instance != null)
			{
				Debug.LogError("[Singleton.cs] Trying to instantiate a second" +
					"instance of a singleton class.");
			}
			else
			{
				Instance = (T)this;
			}

			DontDestroyOnLoad(this);
		}

		protected void OnDestroy()
		{
			if (Instance == this)
			{
				Instance = null;
			}
		}
	}
}