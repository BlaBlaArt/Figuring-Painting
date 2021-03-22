using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] private GameObject[] camersForStages;

    private int StagesNum;

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
        StagesNum = 0;
        SetCamersOutOfStage();

        GameC.Instance.OnLevelUnload += OnLevelUnload;
        GameC.Instance.OnFirstInput += OnFirstInput;
    }

    private void OnFirstInput()
    {
        OnStageComplete();
    }

    private void OnLevelUnload()
    {
        StagesNum = 0;
        SetCamersOutOfStage();
    }

    public void SetCamersOutOfStage()
    {
        foreach (var camera in camersForStages)
        {
            camera.SetInactive();
        }

        camersForStages[StagesNum].SetActive();
    }

    public void OnStageComplete()
    {
        StagesNum++;

        SetCamersOutOfStage();
    }
}    
