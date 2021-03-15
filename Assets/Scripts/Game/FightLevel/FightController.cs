using System.Collections;
using System.Collections.Generic;
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
        LevelController.Instance.OnStageStart += OnStageStart;
    }

    private void OnDestroy()
    {
        GameC.Instance.OnLevelUnload -= OnLevelUnload;
        LevelController.Instance.OnStageStart -= OnStageStart;
    }

    private void OnStageStart(int obj)
    {
        TmpObjects.Clear();
    }

    private void OnLevelUnload()
    {
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
