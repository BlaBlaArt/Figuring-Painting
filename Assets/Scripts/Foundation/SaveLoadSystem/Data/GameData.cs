using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
	public StoredValue<int> Level;
	public StoredValue<int> LevelUI;
	public StoredValue<int> Coins;
	public StoreCharacter StoredCharacters;

	public StoredValue<bool> TutorialShown;

	public GameData()
	{
		StoredCharacters = new StoreCharacter();
		
		LevelUI = new StoredValue<int>(1);
		Level = new StoredValue<int>(0);
		Coins = new StoredValue<int>(0);
		TutorialShown = new StoredValue<bool>();
	}
	
}

[Serializable]
public class StoreCharacter
{
	public List<Characters> StoregeCharacters;
	
	public StoreCharacter()
	{
		StoregeCharacters = new List<Characters>()
		{
			new Characters(CharacterClass.Archer),
			new Characters(CharacterClass.Shield),
			new Characters(CharacterClass.Warior),
			new Characters(CharacterClass.Wizard)
		};
	}
}

[Serializable]
public class Characters
{
	public StoredValue<CharacterClass> CharacterClass;
	public StoredValue<int> Counts;

	public Characters(CharacterClass characterClass)
	{
		CharacterClass = new StoredValue<CharacterClass>(characterClass);
		Counts = new StoredValue<int>(0);
	}
	
}