using UnityEditor;
using UnityEngine;

public static class Tools24Play
{
	[MenuItem("Tools/24Play/Clear Level Data")]
	public static void ClearLevelData()
	{
		var saveLoad = new SaveLoadSystem();
		saveLoad.Data.Game.Level.Value = 1;
	}

	[MenuItem("Tools/24Play/Clear Coins Data")]
	public static void ClearCoinslData()
	{
		var saveLoad = new SaveLoadSystem();
		saveLoad.Data.Game.Coins.Value = 0;
	}

	[MenuItem("Tools/24Play/Save FB Setting")]
	public static void SaveFB()
	{
		var fbs = AssetDatabase.LoadAssetAtPath("Assets/FacebookSDK/SDK/Resources/FacebookSettings.asset", typeof(Object));

		if (fbs == null)
		{
			Debug.LogWarning("Can't find file Assets/FacebookSDK/SDK/Resources/FacebookSettings.asset");
			return;
		}

		EditorUtility.SetDirty(fbs);
		AssetDatabase.SaveAssets();
	}
}