using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class FightController : MonoBehaviour
{
    public static FightController Instance;
    
    private GameObject finishButton;

    public GameObject[] Enemys;
    [SerializeField] private Cell[] Cells; 
    
    public List<GameObject> Characters = new List<GameObject>();

    private LevelData levelData;

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
        foreach (var enemy in Enemys)
        {
            enemy.GetComponent<FightCharacterController>().IsHero = false;
        }
        
        finishButton = GameC.Instance.Finish2StageButton;
        levelData = LevelController.Instance.CurrentLevelData;
        finishButton.GetComponent<Button>().onClick.AddListener(OnStageComplete);
    }

    private void OnStageComplete()
    {
        Debug.Log("2 stagte Complete");
        DisactiveCells();
        GetCharacters();
        StartFight();
        
        finishButton.GetComponent<Button>().onClick.RemoveAllListeners();
        finishButton.SetInactive();

    }

    private void DisactiveCells()
    {
        foreach (var cell in Cells)
        {
            cell.gameObject.SetInactive();
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
                Cells[i].MyObject.GetComponent<FightCharacterController>().IsHero = true;
            }
        }
    }
}
