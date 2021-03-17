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
    public Cell[] Cells;
    [SerializeField] private Cell[] enemyCells;
    
    public List<GameObject> Characters = new List<GameObject>();

    private GameObject ArcherPref, WariorPref, WizardPref, ShieldPref;
    private GameObject ArcherEnemyPref, WariorEnemyPref, WizardEnemyPref, ShieldEnemyPref;
    
    private LevelData levelData;

    private UnityAction FightButton;

    private LevelData currentLevelData;
    
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
        currentLevelData = LevelController.Instance.CurrentLevelData;
        var data = GameC.Instance.AllLevelData;
        ArcherPref = data.ArcherPref;
        WariorPref = data.WariorPref;
        WizardPref = data.WizardPref;
        ShieldPref = data.ShieldPref;

        ArcherEnemyPref = data.ArcherEnemyPref;
        WariorEnemyPref = data.WariorEnemyPref;
        WizardEnemyPref = data.WizardEnemyPref;
        ShieldEnemyPref = data.ShieldEnemyPref;

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
        
        SpawnEnemies();
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

    public void OnDragObjectDown()
    {
        this.WaitAndDoCoroutine(0.15f, () =>
        {
            foreach (var cell in Cells)
            {
                cell.OnDisactive();
            }
        });
    }
    
    private void SpawnEnemies()
    {
        int heroCount = 0;
        int enemyCounts = 0;
        
        foreach (var character in currentLevelData.CurretLevelCharacters)
        {
            if (character.count > 0)
                heroCount+= character.count;
        }
        var data = SLS.Data.Game.StoredCharacters.StoregeCharacters.FindAll((sc => sc.Counts.Value > 0));
        foreach (var character in data)
        {
            heroCount += character.Counts.Value;
        }

        if (SLS.Data.Game.Level.Value == 0)
        {
            enemyCounts = --heroCount;
        }
        else
        {
            enemyCounts = heroCount;
        }

        Debug.Log("EnemyCounts" + enemyCounts);
        
        SpawnEnemiesSetCell(enemyCounts);
    }

    private void SpawnEnemiesSetCell(int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            var cell = GetCell(enemyCells,true);
            GameObject tmpEnemy = null;
            
            if (cell.Id < 3)
            {
                tmpEnemy = Instantiate(ChooseEnemy(CharacterClass.Shield, CharacterClass.Warior));
            }
            else if (cell.Id >=3 && cell.Id <6)
            {
                tmpEnemy = Instantiate(ChooseEnemy());
            }
            else if (cell.Id >= 6 && cell.Id < 9)
            {
                tmpEnemy = Instantiate(ChooseEnemy(CharacterClass.Wizard));
            }
            
            tmpEnemy.transform.position = cell.transform.position;
            tmpEnemy.transform.rotation = cell.transform.rotation;
        
            cell.OnGetObject(tmpEnemy);
            Enemys.Add(tmpEnemy);
        }

        foreach (var enemyCell in enemyCells)
        {
            enemyCell.enabled = false;
        }
    }

    private GameObject ChooseEnemy()
    {
        List<CharacterClass> characters = new List<CharacterClass>
        {
            CharacterClass.Archer,
            CharacterClass.Shield,
            CharacterClass.Warior,
            CharacterClass.Wizard
        };
        
        var characterClass = characters.GetRandom();

        switch (characterClass)
        {
            case CharacterClass.Archer:
            {
                return ArcherEnemyPref;
            }
            case CharacterClass.Shield:
            {
                return ShieldEnemyPref;
            }
            case CharacterClass.Warior:
            {
                return WariorEnemyPref;
            }
            case CharacterClass.Wizard:
            {
                return WizardEnemyPref;
            }
            default:
                return null;
        }
        
    }
    
    private GameObject ChooseEnemy(CharacterClass enemyClassToNotSpawn)
    {
        List<CharacterClass> characters = new List<CharacterClass>
        {
            CharacterClass.Archer,
            CharacterClass.Shield,
            CharacterClass.Warior,
            CharacterClass.Wizard
        };

        var extra = characters.First(t => t == enemyClassToNotSpawn);
        characters.Remove(extra);

        var characterClass = characters.GetRandom();

        switch (characterClass)
        {
            case CharacterClass.Archer:
            {
                return ArcherEnemyPref;
            }
            case CharacterClass.Shield:
            {
                return ShieldEnemyPref;
            }
            case CharacterClass.Warior:
            {
                return WariorEnemyPref;
            }
            case CharacterClass.Wizard:
            {
                return WizardEnemyPref;
            }
            default:
                return null;
        }
        
    }

    private GameObject ChooseEnemy(CharacterClass enemyClassToNotSpawn1, CharacterClass enemyClassToNotSpawn2)
    {
        List<CharacterClass> characters = new List<CharacterClass>
        {
            CharacterClass.Archer,
            CharacterClass.Shield,
            CharacterClass.Warior,
            CharacterClass.Wizard
        };

        var extra1 = characters.First(t => t == enemyClassToNotSpawn1);
        var extra2 = characters.First(t => t == enemyClassToNotSpawn2);
        characters.Remove(extra1);
        characters.Remove(extra2);

        var characterClass = characters.GetRandom();

        switch (characterClass)
        {
            case CharacterClass.Archer:
            {
                return ArcherEnemyPref;
            }
            case CharacterClass.Shield:
            {
                return ShieldEnemyPref;
            }
            case CharacterClass.Warior:
            {
                return WariorEnemyPref;
            }
            case CharacterClass.Wizard:
            {
                return WizardEnemyPref;
            }
            default:
                return null;
        }
        
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
                var cell = GetCell(Cells,false);
                if (cell != null)
                {
                    var tmpCharacter = Instantiate(GetCharacter(type));
                    Debug.Log("Cell " + cell.name + " Character " + tmpCharacter.name);
                    tmpCharacter.transform.position = cell.transform.position;
                    tmpCharacter.transform.rotation = cell.transform.rotation;
                    tmpCharacter.GetComponent<DragObject>().OnSpawnOnCell(cell);
                    tmpCharacter.GetComponent<DragObject>().startCell = cell;
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
    
    public Cell GetCell(Cell[] cells, bool isShofle)
    {

        if (cells.All(c => c.MyObject != null))
        {
            return null;
        }
        else
        {
            if (!isShofle)
            {
                var cell = cells.First(c => c.MyObject == null);
                return cell; 
            }
            else
            {
                cells.Shuffle();
                var cell = cells.First(c => c.MyObject == null);
                return cell;
            }
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
