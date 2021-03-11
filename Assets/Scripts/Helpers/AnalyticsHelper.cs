//using Facebook.Unity;
//using GameAnalyticsSDK;
using UnityEngine;

public static class AnalyticsHelper
{
	static bool _levelWasStarted;

	static int GetCurrentLevelIndex()
	{
		return PlayerPrefs.GetInt("AH_GA_Level", 1);
	}

	static string GetCurrentLevelName()
	{
		return string.Format("Level_{0}", GetCurrentLevelIndex());
	}

	static void IncrementLevel()
	{
		int _cur_level = GetCurrentLevelIndex();

		PlayerPrefs.SetInt("AH_GA_Level", _cur_level + 1);

		PlayerPrefs.Save();
	}

	public static void Init()
	{
		// GameAnalytics.Initialize();

		// if (FB.IsInitialized)
		// {
		// 	FB.ActivateApp();
		// }
		// else
		// {
		// 	FB.Init(() =>
		// 	{
		// 		FB.ActivateApp();
		// 	});
		// }

		Logwin.Log("Init", "", "Analytics", LogwinParam.Color(Color.green));
	}

	public static void StartLevel()
	{
		_levelWasStarted = true;

		Logwin.Log("StartLevel", GetCurrentLevelName(), "Analytics", LogwinParam.Color(Color.green));

		// GameAnalytics.NewProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Start, "Game", GetCurrentLevelName());
	}

	public static void CompleteLevel()
	{
		if (!_levelWasStarted) return;

		Logwin.Log("CompleteLevel", GetCurrentLevelName(), "Analytics", LogwinParam.Color(Color.green));

		// GameAnalytics.NewProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Complete, "Game", GetCurrentLevelName());

		IncrementLevel();

		_levelWasStarted = false;
	}

	public static void FailLevel()
	{
		if (!_levelWasStarted) return;

		Logwin.Log("FailLevel", GetCurrentLevelName(), "Analytics", LogwinParam.Color(Color.green));

		// GameAnalytics.NewProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Fail, "Game", GetCurrentLevelName());

		_levelWasStarted = false;
	}
}