using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Data", menuName = "Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    public List<Character> CurretLevelCharacters;
}

[Serializable]
public struct Character
{
    public CharacterClass CharacterClass;
    public int count;
    public GameObject CharacterPref;
}
