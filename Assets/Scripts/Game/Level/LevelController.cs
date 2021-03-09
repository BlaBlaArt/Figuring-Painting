using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance;

    public AssemblyController[] assemblyControllers;
    public UnboxController unboxController;

    public LevelData CurrentLevelData;

    public int StageNum;

    private int assemblyCount;

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
        assemblyCount = 0;
        StageNum = 0;
        
        PartController.OnGrabStart = new UnityEngine.Events.UnityEvent<PartController>();
        PartController.OnGrabStop = new UnityEngine.Events.UnityEvent<PartController>();
        
        if (unboxController)
        {
            unboxController.onBoxOpen.AddListener(OnBoxOpen);
        }
        
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
                Destroy(unboxController.gameObject);
            });
        }
    }
    
    public void OnAssemblyComplete()
    {
        assemblyCount++;
        if (assemblyCount == assemblyControllers.Length)
        {
            OnStageComplete(0);
            Debug.Log("StageComplete");
        }
    }

    private void OnStageComplete(int stageNum)
    {
        StageNum = ++stageNum;
        OnStageStart?.Invoke(StageNum);
        CameraController.Instance.OnStageComplete();
    }

}
