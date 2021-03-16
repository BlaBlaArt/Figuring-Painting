using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class DragController : MonoBehaviour
{
    public static DragController Instance;
    
    private Camera mainCamera;
    
    private GameObject objectToDrag;
    [HideInInspector]
    public float DeltaHeightOfLevel;
    [HideInInspector]
    public float OffsetToForward;
    [HideInInspector]
    public float FollowSmooth;
    
    private LayerMask planeForCamera;

    private bool isDrag = false;
    [HideInInspector]
    public float CellsHeight;
    
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
        var data = GameC.Instance.AllLevelData;
        DeltaHeightOfLevel = data.DeltaHeightOfLevel;
        OffsetToForward = data.OffsetToForward;
        FollowSmooth = data.FollowSmooth;
        planeForCamera = data.PlaneForCamera;
        CellsHeight = data.CellsHeight;


        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (isDrag)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit,50, planeForCamera))
            {
                
                objectToDrag.transform.position =
                    new Vector3(hit.point.x, objectToDrag.transform.position.y, hit.point.z);
            }
        }
    }

    public void SetObjectToDrag(GameObject objectToDrag)
    {
        isDrag = true;
        this.objectToDrag = objectToDrag;
    }

    public void DisableDrag()
    {
        isDrag = false;
    }
    
}
