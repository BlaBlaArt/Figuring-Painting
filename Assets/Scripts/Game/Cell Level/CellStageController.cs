using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class CellStageController : MonoBehaviour
{
    [SerializeField] private Transform[] PointsToSpawn = new Transform[4];

    [SerializeField] private TextMeshPro[] texts = new TextMeshPro[4];
    
    private int CountOfCharecters = 4;
    
    [SerializeField] private int[] counts;

    private GameObject FinishButton;
    
    private LevelData currentLevelData;
    
    private void Start()
    {
        for (int i = 0; i < PointsToSpawn.Length; i++)
        {
            texts[i] = PointsToSpawn[i].GetComponentInChildren<TextMeshPro>();
            texts[i].SetInactive();
        }
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
            texts[numInArray].gameObject.SetActive();
            texts[numInArray].text = counts[numInArray].ToString();
            counts[numInArray]--;
            var tmpCharacter = Instantiate(character.CharacterPref);
            tmpCharacter.transform.position = point.position;
            tmpCharacter.GetComponent<CharacterData>().CharacterNum = numInArray;
        }
        else if (FinishConritionCheck())
        {
            texts[numInArray].gameObject.SetInactive();
            StageComplite();
        }
        else
        {
            texts[numInArray].gameObject.SetInactive();
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
