using Cinemachine;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellStageController : MonoBehaviour
{
    [SerializeField] private Transform[] PointsToSpawn = new Transform[4];

    [SerializeField] private TextMeshPro[] texts = new TextMeshPro[4];
    
    private int CountOfCharecters = 4;
    
    [SerializeField] private int[] counts; 
    private float offsetToSpawn;
    
    private LevelData currentLevelData;
    
    private void Start()
    {
        offsetToSpawn = GameC.Instance.AllLevelData.OffsetToSpawnCharacters;
        GameC.Instance.Finish2StageButton.GetComponent<Button>().onClick.AddListener(OnStartFightStage);
        for (int i = 0; i < PointsToSpawn.Length; i++)
        {
            texts[i] = PointsToSpawn[i].GetComponentInChildren<TextMeshPro>();
            texts[i].SetInactive();
        }
        counts = new int[CountOfCharecters];
        LevelController.Instance.OnStageStart += OnStageStart;
        LevelController.Instance.OnSpawnCharacter += OnSpawnCharacter;
        currentLevelData = LevelController.Instance.CurrentLevelData;
    }

    private void OnStartFightStage()
    {
        GameC.Instance.OnDisableTutorials?.Invoke();
        
        foreach (var text in texts)
        {
            text.gameObject.SetInactive();
        }
    }
    
    private void OnStageStart(int obj)
    {
        if (obj == 1)
        {
            //Debug.Log("CellStage");
            OnStartStage();
        }
    }

    private void OnStartStage()
    {
        for (int i = 0; i < currentLevelData.CurretLevelCharacters.Count; i++)
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

        for (int i = 0; i < counts[numInArray]; i++)
        {
           // counts[numInArray]--;

           var cell = FightController.Instance.GetCell(FightController.Instance.Cells, false);

           if (cell != null)
           {
               var tmpCharacter = Instantiate(character.CharacterPref);
               
               tmpCharacter.transform.position = cell.transform.position;
               tmpCharacter.transform.rotation = point.rotation;
            
               tmpCharacter.GetComponent<CharacterData>().CharacterNum = numInArray;
               tmpCharacter.GetComponent<DragObject>().OnSpawnOnCell(cell);
               //tmpCharacter.GetComponent<DragObject>().startCell = cell;
               cell.OnGetObject(tmpCharacter);
           }
           else
           {
               break;
           }
           
        }
        
        // if (counts[numInArray] > 0)
        // {
        //   //  texts[numInArray].gameObject.SetActive();
        //    // texts[numInArray].text = counts[numInArray].ToString();
        //     counts[numInArray]--;
        //     var tmpCharacter = Instantiate(character.CharacterPref);
        //     
        //     var cell = FightController.Instance.GetCell(FightController.Instance.Cells, false);
        //     
        //     tmpCharacter.transform.position = cell.transform.position;
        //         //new Vector3(point.position.x, point.position.y, point.position.z + offsetToSpawn);
        //     tmpCharacter.transform.rotation = point.rotation;
        //     
        //     tmpCharacter.GetComponent<CharacterData>().CharacterNum = numInArray;
        //   //  FightController.Instance.TmpObjects.Add(tmpCharacter);
        //   //  tmpCharacter.GetComponent<DragObject>().OnStartInvoke(offsetToSpawn);
        //     tmpCharacter.GetComponent<DragObject>().OnSpawnOnCell(cell);
        //     cell.OnGetObject(tmpCharacter);
        // }
        //else if (FinishConritionCheck())
        //{
        //    texts[numInArray].gameObject.SetInactive();
        //}
        //else
        //{
        //    texts[numInArray].gameObject.SetInactive();
        //}
        
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

    private void OnDestroy()
    {
        LevelController.Instance.OnStageStart -= OnStageStart;
        GameC.Instance.Finish2StageButton.GetComponent<Button>().onClick.RemoveListener(OnStartFightStage);
    }

    [Button]
    private void StartStage()
    {
        OnStartStage();
        CameraController.Instance.OnStageComplete();
    }

}
