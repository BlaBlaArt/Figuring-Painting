using UnityEngine;

public abstract class MonoSingletons<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T Instance
	{
		get
		{
			if (Instance == null)
				Debug.Log($"{typeof(T).ToString()} MonoSingletons is null");

			return Instance;
		}

		private set => Instance = value;
	}

	private void Awake()
	{
		if (Instance == null)
			Instance = this as T;
	}
}