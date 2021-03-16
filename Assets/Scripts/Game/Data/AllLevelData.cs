using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "All level Data", fileName = "New All level Data", order = 1)]
public class AllLevelData : ScriptableObject
{
    [Header("AssembleStage")]
    public float HeightOFBoxOffset;
    public float DeltaTimeToMoveBox;
    

    [Header("CellStage")]
    public float OffsetToSpawnCharacters;

    
    [Header("DragStage")]
    public float DeltaHeightOfLevel;
    public float OffsetToForward;
    public float FollowSmooth;
    public LayerMask PlaneForCamera;
    public float CellsHeight;
    
    
    [Header("FightStage")]
    public GameObject ArcherPref, WariorPref, WizardPref, ShieldPref;


}
