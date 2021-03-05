using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance;

    public AssemblyController[] assemblyControllers;
    public UnboxController unboxController;

    private int assemblyCount;

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
        
        PartController.onGrabStart = new UnityEngine.Events.UnityEvent<PartController>();
        PartController.onGrabStop = new UnityEngine.Events.UnityEvent<PartController>();
        
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
            OnStageComplete();
            Debug.Log("StageComplete");
        }
    }

    private void OnStageComplete()
    {
        CameraController.Instance.OnStageComplete();
    }

}
