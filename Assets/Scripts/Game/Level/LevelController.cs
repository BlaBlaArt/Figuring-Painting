using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance;

   // [SerializeField] private GameObject SecondStagePref;
    [SerializeField] private float HeightOffset;
    [SerializeField] private float deltaTimeMove;
    
    public AssemblyController[] assemblyControllers;
    public UnboxController unboxController;

    public LevelData CurrentLevelData;

    public int StageNum;

    private int assemblyCount;

    private List<GameObject> objectsToMove = new List<GameObject>();
    private List<Vector3> startPositions = new List<Vector3>();

    private GameObject tmpLevel;
    
    public event Action<int> OnStageStart;
    public Action<int> OnSpawnCharacter;
    
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
        objectsToMove = new List<GameObject>();
        startPositions = new List<Vector3>();
        assemblyCount = 0;
        StageNum = 0;
        
        PartController.OnGrabStart = new UnityEngine.Events.UnityEvent<PartController>();
        PartController.OnGrabStop = new UnityEngine.Events.UnityEvent<PartController>();
        
        if (unboxController)
        {
            objectsToMove.Add(unboxController.gameObject);
            startPositions.Add(unboxController.transform.position);
            unboxController.transform.position += Vector3.up * HeightOffset;
            unboxController.onBoxOpen.AddListener(OnBoxOpen);
        }

        foreach (var assembly in assemblyControllers)
        {
            objectsToMove.Add(assembly.gameObject);
            startPositions.Add(assembly.transform.position);
            assembly.transform.position += Vector3.up * HeightOffset;
        }

        GameC.Instance.OnFirstInput += OnFirstInput;

    }

    private void OnDestroy()
    {
        Destroy(tmpLevel);
        GameC.Instance.OnFirstInput -= OnFirstInput;
    }

    private void OnFirstInput()
    {
        for (int i = 0; i < objectsToMove.Count; i++)
        {
            objectsToMove[i].transform.DOMove(startPositions[i], deltaTimeMove).SetEase(Ease.InCubic);
        }

        this.WaitAndDoCoroutine(deltaTimeMove, () =>
        {
            GameC.Instance.OnShowTutorial?.Invoke(1);
        });
    }

    void OnBoxOpen()
    {
        foreach (var controller in assemblyControllers)
        {
            controller.ReadyToAssemble();
            this.WaitAndDoCoroutine(1.5f,() =>
            {
                controller.PartPlacement();
                unboxController.onBoxOpen.RemoveListener(OnBoxOpen);
                GameC.Instance.OnAssembleStage?.Invoke();
                Destroy(unboxController.gameObject);
            });
        }
    }
    
    public void OnAssemblyComplete()
    {
        assemblyCount++;
        if (assemblyCount == assemblyControllers.Length)
        {
            foreach (var assembly in assemblyControllers)
            {
                assembly.OnAllAssembleComplete();
            }
            OnStageComplete(0);
            Debug.Log("StageComplete");
        }
    }

    private void OnStageComplete(int stageNum)
    {
      //  tmpLevel = Instantiate(SecondStagePref);
      GameC.Instance.Load2Stage?.Invoke();
      
        this.WaitAndDoCoroutine(0.5f, () =>
        {
            StageNum = ++stageNum;
            OnStageStart?.Invoke(StageNum);
            CameraController.Instance.OnStageComplete();
            GameC.Instance.OnShowTutorial?.Invoke(3);
        });
    }

}
