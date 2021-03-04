using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragController : MonoBehaviour
{
    private Camera mainCamera;
    
    private GameObject objectToDrag;
    
    [SerializeField] private LayerMask planeForCamera;

    private bool isDrag = false;

    public Vector3 Hit;
    
    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (isDrag)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit,50, planeForCamera))
            {
                Hit = hit.point;
                
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
