using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    public CharacterClass myCharacterClass;
    public int CharacterNum;
}

public enum CharacterClass
{
    Warior,
    Archer
}
