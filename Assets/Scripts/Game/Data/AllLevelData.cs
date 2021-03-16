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
    
    
    [Header("FightStageHero")]
    public GameObject ArcherPref, WariorPref, WizardPref, ShieldPref;
    
    [Header("FightStageEnemy")]
    public GameObject ArcherEnemyPref, WariorEnemyPref, WizardEnemyPref, ShieldEnemyPref;


}
