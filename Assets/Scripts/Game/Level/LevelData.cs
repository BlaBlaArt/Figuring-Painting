using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Data", menuName = "Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    public Characters CurretLevelCharacters;
}

[Serializable]
public struct Characters
{
    public int[,] countsByType;
    
}
