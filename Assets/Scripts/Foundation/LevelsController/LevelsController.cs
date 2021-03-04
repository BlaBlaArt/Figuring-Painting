using UnityEngine;

public class LevelsController : MonoBehaviour
{
	public static LevelsController Instance { get; private set; }

	private ILevelProvider levelProvider;

	public int LevelsCount => levelProvider != null ? levelProvider.LevelsCount : 0;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;

		levelProvider = GetComponent<ILevelProvider>();
		if (levelProvider == null)
			Debug.LogWarning("Провайдер не выбран! Так это не работает", this);
	}

	private void Start()
	{
		GameC.Instance.OnLevelStart += LoadLevel;
	}

	private void OnDestroy()
	{
		GameC.Instance.OnLevelStart -= LoadLevel;
	}

	private void LoadLevel(int levelNumber) => levelProvider?.LoadLevel(levelNumber % levelProvider.LevelsCount);
	private void Reload() => levelProvider?.Reload();
}