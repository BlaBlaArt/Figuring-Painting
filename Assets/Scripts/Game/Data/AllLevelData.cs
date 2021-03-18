using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "All level Data", fileName = "New All level Data", order = 1)]
public class AllLevelData : ScriptableObject
{
    [BoxGroup("Assemble Stage")]
    public float HeightOFBoxOffset;
    [BoxGroup("Assemble Stage")]
    public float DeltaTimeToMoveBox;
    

    [BoxGroup("Cell Stage")]
    public float OffsetToSpawnCharacters;
    [BoxGroup("Cell Stage")]
    public Color StartCellColor, ActiveCellColor;

    
    [BoxGroup("Drag Stage")]
    public float DeltaHeightOfLevel;
    [BoxGroup("Drag Stage")]
    public float OffsetToForward;
    [BoxGroup("Drag Stage")]
    public float FollowSmooth;
    [BoxGroup("Drag Stage")]
    public LayerMask PlaneForCamera;
    [BoxGroup("Drag Stage")]
    public float CellsHeight;
    
    
    [BoxGroup("Fight Stage Hero")] [ShowAssetPreview(128,128)]
    public GameObject ArcherPref, WariorPref, WizardPref, ShieldPref;
    
    [BoxGroup("Fight Stage Enemies")] [ShowAssetPreview(128,128)]
    public GameObject ArcherEnemyPref, WariorEnemyPref, WizardEnemyPref, ShieldEnemyPref;


}
