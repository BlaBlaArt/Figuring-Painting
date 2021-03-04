using System;

[Serializable]
public class SaveLoadData
{
	public GameData Game;
	public SettingsData Settings;
	public TutorialData TutorialData;

	public SaveLoadData()
	{
		Game = new GameData();
		Settings = new SettingsData();
		TutorialData = new TutorialData();
	}
}