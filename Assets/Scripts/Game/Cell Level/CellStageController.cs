using NaughtyAttributes;
using UnityEngine;

public class CellStageController : MonoBehaviour
{
    [SerializeField] private Transform[] PointsToSpawn = new Transform[4];

    [SerializeField] private int[] counts = new int[4];
    
    private LevelData currentLevelData;
    
    private void Start()
    {
        LevelController.Instance.OnStageStart += OnStageStart;
        LevelController.Instance.OnSpawnCharacter += OnSpawnCharacter;
        currentLevelData = LevelController.Instance.CurrentLevelData;
    }

    private void OnStageStart(int obj)
    {
        if (obj == 1)
        {
            Debug.Log("CellStage");
            OnStartStage();
        }
    }

    private void OnStartStage()
    {
        for (int i = 0; i < currentLevelData.CurretLevelCharacters.Length; i++)
        {
            counts[i] = currentLevelData.CurretLevelCharacters[i].count;
            SpawnCharacters(currentLevelData.CurretLevelCharacters[i], PointsToSpawn[i],i);
        }
    }

    private void OnSpawnCharacter(int CharacterNum)
    {
        SpawnCharacters(currentLevelData.CurretLevelCharacters[CharacterNum], PointsToSpawn[CharacterNum],CharacterNum);
    }

    private void SpawnCharacters(Character character, Transform point, int numInArray)
    {
        if (counts[numInArray] > 0)
        {
            counts[numInArray]--;
            var tmpCharacter = Instantiate(character.CharacterPref);
            tmpCharacter.transform.position = point.position;
            tmpCharacter.GetComponent<CharacterData>().CharacterNum = numInArray;
        }
    }
    
    private void OnDestroy()
    {
        LevelController.Instance.OnStageStart -= OnStageStart;
    }

    [Button]
    private void StartStage()
    {
        OnStartStage();
    }
}
