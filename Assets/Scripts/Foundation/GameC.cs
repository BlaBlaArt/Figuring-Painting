using System;
using UnityEngine;

public class GameC : MonoBehaviour
{
	public static GameC Instance { get; private set; }


	public Action OnFirstInput;

	public Action OnAssembleStage;

	public Action<int> OnShowTutorial;
	public event Action OnInitComplite;
	public event Action<int> OnLevelStart;
	public event Action<bool> OnLevelEnd;

	public event Action OnLevelUnload;


	public GameObject Finish2StageButton;

	public Camera MainCamera;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;

		MainCamera = Camera.main;
	}

	private void Start()
	{
		Finish2StageButton.SetInactive();
		
		Application.targetFrameRate = 60;

		OnInitComplite += AnalyticsHelper.Init;

		this.DoAfterNextFrameCoroutine(() =>
		{
			OnInitComplite?.Invoke();
			LoadLevel();
		});
	}

	public void ShowFinishButton()
	{
		if (!Finish2StageButton.activeSelf)
		{
			Finish2StageButton.SetActive();
		}
	}
	
	public void LoadLevel()
	{
		var levelNumber = SLS.Data.Game.Level.Value;

		AnalyticsHelper.StartLevel();
		OnLevelStart?.Invoke(levelNumber);
	}

	private void UnloadLevel(bool nextLvl)
	{
		Finish2StageButton.SetInactive();

		if (nextLvl)
			SLS.Data.Game.Level.Value++;

		OnLevelUnload?.Invoke();
	}

	public void LevelEnd(bool playerWin)
	{
		if (playerWin)
			AnalyticsHelper.CompleteLevel();
		else
			AnalyticsHelper.FailLevel();

		OnLevelEnd?.Invoke(playerWin);
	}

	public void NextLevel()
	{
		UnloadLevel(true);
		LoadLevel();
	}

	public void RestartLevel()
	{
		UnloadLevel(false);
		LoadLevel();
	}
}