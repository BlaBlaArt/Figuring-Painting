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
			new Characters(0), // Warior
			new Characters(1), // Archer
			new Characters(2), // Wizard
			new Characters(3) // Shield
		};
	}
}

[Serializable]
public class Characters
{
	public StoredValue<int> CharacterClass;
	public StoredValue<int> Counts;

	public Characters(int numb)
	{
		//int num = GetCharacterID(characterClass);
		
		CharacterClass = new StoredValue<int>(numb);
		Counts = new StoredValue<int>(0);
	}

	//public int GetCharacterID(CharacterClass characterClass)
	//{
	//	switch (characterClass)
	//	{
	//		case global::CharacterClass.Warior:
	//		{
	//			return 0;
	//		}
	//		case global::CharacterClass.Archer:
	//		{
	//			return 1;
	//		}
	//		case global::CharacterClass.Wizard:
	//		{
	//			return 2;
	//		}
	//		case global::CharacterClass.Shield:
	//		{
	//			return 3;
	//		}
	//		default:
	//		{
	//			return 10;
	//		}
	//	}
	//}
}