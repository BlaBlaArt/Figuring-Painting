using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FightController : MonoBehaviour
{
    public static FightController Instance;
    
    private GameObject finishButton;

    public List<GameObject> TmpObjects;

    public List<GameObject> Enemys = new List<GameObject>();
    [SerializeField] private Cell[] Cells;
    [SerializeField] private GameObject[] enemyCells;
    
    public List<GameObject> Characters = new List<GameObject>();

    [SerializeField] private GameObject ArcherPref, WariorPref, WizardPref, ShieldPref;
    
    private LevelData levelData;

    private UnityAction FightButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        FightButton = new UnityAction(OnStageComplete);
        
        foreach (var enemy in Enemys)
        {
            enemy.GetComponent<FightCharacterController>().IsHero = false;
        }
        
        finishButton = GameC.Instance.Finish2StageButton;
        levelData = LevelController.Instance.CurrentLevelData; 
        finishButton.GetComponent<Button>().onClick.AddListener(FightButton);

        GameC.Instance.OnLevelUnload += OnLevelUnload;
        GameC.Instance.OnLevelEnd += OnLevelEnd;
        LevelController.Instance.OnStageStart += OnStageStart;
    }

    private void OnLevelEnd(bool isWin)
    {
        if (isWin)
        {
            Debug.Log(Characters.Print());
            
            SLS.Data.Game.StoredCharacters.StoregeCharacters.ForEach((sc => sc.Counts.Value = 0));
            foreach (var character in Characters)
            {
                var data = character.GetComponent<CharacterData>();
            //    int num = SLS.Data.Game.StoredCharacters.Value[data.myCharacterClass];
            SLS.Data.Game.StoredCharacters.StoregeCharacters
                .Find(d => d.CharacterClass.Value == data.myCharacterClass).Counts.Value++;
          //  SLS.Data.Game.StoredCharacters.Value.Counts[num]++;
                SLS.Save();
                Debug.Log("character class " + data.myCharacterClass + " CharacterData " +
                          SLS.Data.Game.StoredCharacters.StoregeCharacters
                              .Find(d => d.CharacterClass.Value == data.myCharacterClass).Counts.Value);
            }
        }
    }

    private void OnDestroy()
    {
        GameC.Instance.OnLevelEnd -= OnLevelEnd;
        GameC.Instance.OnLevelUnload -= OnLevelUnload;
        LevelController.Instance.OnStageStart -= OnStageStart;
    }

    private void OnStageStart(int obj)
    {
        SpawnCharactersFromData();
        TmpObjects.Clear();
    }

    private void SpawnCharactersFromData()
    {

        var data = SLS.Data.Game.StoredCharacters.StoregeCharacters.FindAll((sc => sc.Counts.Value > 0));
        for (int i = 0; i < data.Count; i++)
        {
            Debug.Log("character class " + data[i].CharacterClass.Value + " CharacterData " +
                      data[i].Counts.Value);
            var type = data[i].CharacterClass.Value;
            for (int j = 0; j < data[i].Counts.Value; j++)
            {
                var cell = GetCell();
                if (cell != null)
                {
                    var tmpCharacter = Instantiate(GetCharacter(type));
                    Debug.Log("Cell " + cell.name + " Character " + tmpCharacter.name);
                    tmpCharacter.transform.position = cell.transform.position;
                    tmpCharacter.transform.rotation = cell.transform.rotation;
                    tmpCharacter.GetComponent<DragObject>().OnSpawnOnCell(cell);
                    cell.OnGetObject(tmpCharacter);
                }
            }
        }
    }

    private GameObject GetCharacter(CharacterClass characterClass)
    {
        switch (characterClass)
        {
            case CharacterClass.Archer:
            {
                return ArcherPref;
            }
            case CharacterClass.Shield:
            {
                return ShieldPref;
            }
            case CharacterClass.Warior:
            {
                return WariorPref;
            }
            case CharacterClass.Wizard:
            {
                return WizardPref;
            }
        }

        return null;
    }
    
    private Cell GetCell()
    {

        if (Cells.All(c => c.MyObject != null))
        {
            return null;
        }
        else
        {
            var cell = Cells.First(c => c.MyObject == null);
            return cell;
        }
    }

    private void OnLevelUnload()
    {
        Debug.Log("levelUnload");
        
        foreach (var enemy in Enemys)
        {
            Destroy(enemy);
        }

        foreach (var character in Characters)
        {
           
            Destroy(character);
        }

        foreach (var tmp in TmpObjects)
        {
            Destroy(tmp);
        }
    }
    
    private void OnStageComplete()
    {
        Debug.Log("2 stagte Complete");
        DisactiveCells();
        GetCharacters();
        DestroyNotUsedCherecters();
        StartFight();
        
      //  finishButton.GetComponent<Button>().onClick.RemoveAllListeners();
        finishButton.GetComponent<Button>().onClick.RemoveListener(FightButton);
        finishButton.SetInactive();

    }

    private void DisactiveCells()
    {

        foreach (var cell in Cells)
        {
            cell.transform.DOMoveY(-2.75f, 0.5f).OnComplete(() =>
            {
                cell.gameObject.SetInactive();
            });
        }

        foreach (var cell in enemyCells)
        {
            cell.transform.DOMoveY(-2.75f, 0.5f).OnComplete(() =>
            {
                cell.gameObject.SetInactive();
            });
        }
    }
    
    private void StartFight()
    {
        foreach (var enemy in Enemys)
        {
            enemy.GetComponent<FightCharacterController>().StartFight();
        }

        foreach (var character in Characters)
        {
            character.GetComponent<FightCharacterController>().StartFight();
        }
    }

    private void GetCharacters()
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            if (Cells[i].MyObject != null)
            {
                Characters.Add(Cells[i].MyObject);
                TmpObjects.Remove(Cells[i].MyObject);
                Cells[i].MyObject.GetComponent<FightCharacterController>().IsHero = true;
            }
        }
    }

    private void DestroyNotUsedCherecters()
    {
        foreach (var tmp in TmpObjects)
        {
            Destroy(tmp);
        }
        
        TmpObjects.Clear();
    }

    public void StartWinCheck()
    {
        StartCoroutine(WinCheck());
    }
    
    private IEnumerator WinCheck()
    {
        yield return new WaitForSeconds(0.25f);
        
        if (Enemys.Count == 0 && Characters.Count == 0)
        {
            GameC.Instance.LevelEnd(false);

        }
 
    }
}
