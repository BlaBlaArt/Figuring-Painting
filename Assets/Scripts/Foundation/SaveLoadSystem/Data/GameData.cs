using System;

[Serializable]
public class GameData
{
	public StoredValue<int> Level;
	public StoredValue<int> LevelUI;
	public StoredValue<int> Coins;

	public StoredValue<bool> TutorialShown;

	public GameData()
	{
		LevelUI = new StoredValue<int>(1);
		Level = new StoredValue<int>(0);
		Coins = new StoredValue<int>(0);
		TutorialShown = new StoredValue<bool>();
	}
}