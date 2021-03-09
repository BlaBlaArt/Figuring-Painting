using NaughtyAttributes;
using UnityEngine;

public class CellStageController : MonoBehaviour
{
    [SerializeField] private Transform[] PointsToSpawn = new Transform[4];

    private int CountOfCharecters = 4;
    
    [SerializeField] private int[] counts;

    private GameObject FinishButton;
    
    private LevelData currentLevelData;
    
    private void Start()
    {
        counts = new int[CountOfCharecters];
        FinishButton = GameC.Instance.Finish2StageButton;
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
        else if (FinishConritionCheck())
        {
            StageComplite();
        }
        
    }

    private bool FinishConritionCheck()
    {
        int check = 0;

        for (int i = 0; i < counts.Length; i++)
        {
            if (counts[i] == 0)
                check++;
        }
        
        if (check == CountOfCharecters)
            return true;
        else
            return false;

    }

    private void StageComplite()
    {
        FinishButton.SetActive();
    }
    
    private void OnDestroy()
    {
        LevelController.Instance.OnStageStart -= OnStageStart;
    }

    [Button]
    private void StartStage()
    {
        OnStartStage();
        CameraController.Instance.OnStageComplete();
    }

}
