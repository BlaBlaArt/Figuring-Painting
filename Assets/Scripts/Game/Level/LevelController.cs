using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public AssemblyController assemblyController;
    public UnboxController unboxController;


    private void Start()
    {
        print("FYYUOHOU");

        PartController.onGrabStart = new UnityEngine.Events.UnityEvent<PartController>();
        PartController.onGrabStop = new UnityEngine.Events.UnityEvent<PartController>();
        
        if (unboxController)
        {
            // assemblyController.gameObject.SetInactive();
            // _ucStartPos = unboxController.transform.position.x;
            unboxController.onBoxOpen.AddListener(OnBoxOpen);
        }
        
        // unboxController.transform.DOMoveX(0f, objectMoveTime).OnComplete(Help);
        // void Help()
        // {
        //     assemblyController.gameObject.SetActive();
        //     assemblyController.PartPlacement();
        // }
        
        // assemblyController.onAssemblyFinished.AddListener(OnAssemblyFinished);
    }
    
    void OnBoxOpen()
    {
        // unboxCam.Priority = 0;
        // mainCam.Priority = 1;

        // TutorialController.inst.ShowTutorialAssembly();
        assemblyController.ReadyToAssemble();
        this.WaitAndDoCoroutine(1.5f,() => assemblyController.PartPlacement());
        // this.Invoke(unboxController.openAnimDelay, assemblyController.ReadyToAssemble);
    }
    
    // void OnAssemblyFinished()
    // {
    //     targetController.MoveIn();
    //
    //     TutorialController.inst.ShowTutorialGunplay();
    // }
}
