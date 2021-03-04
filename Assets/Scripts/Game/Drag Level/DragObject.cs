using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    [SerializeField] private DragController dragController;

   // [SerializeField] private Transform MyPerfectPosition;

   [SerializeField] private LayerMask CellLayer;
   
    [SerializeField] private float HeightOfLevel;

    private CharacterData myData;
    
    private Cell cell;

    private Vector3 startPos;

    private void Start()
    {
        myData = GetComponent<CharacterData>();
        startPos = transform.position;
    }

    private void OnMouseDown()
    {
        dragController.SetObjectToDrag(gameObject);
        if (cell != null)
        {
            cell.OnObjectMove();
        }
        
        Debug.Log("DOWN");
    }

    private void OnMouseDrag()
    {
        transform.position = new Vector3(transform.position.x, HeightOfLevel, transform.position.z);
    }

    private void OnMouseUp()
    {
        dragController.DisableDrag();

        CheckCell();
    }

    private void CheckCell()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit,50, CellLayer))
        {
            Debug.Log("Cell");
            cell = hit.transform.GetComponent<Cell>();

            if (cell.MyObject != null)
            {
                Merge(cell.MyObject);
            }
            else
            {
                transform.position = cell.StartPos;
                cell.OnGetObject(gameObject);
            }
        }
        else
        {
            transform.position = startPos;
        }
    }

    private void Merge(GameObject obj)
    {
        var data = obj.GetComponent<CharacterData>();
        CheckLevelObjectOnCell(obj, data.MyLevel);
    }

    private void CheckLevelObjectOnCell(GameObject obj, Levels levels)
    {
        if (myData.MyLevel == levels)
        {
            switch (levels)
            {
                case Levels.Warior:
                {
                    Destroy(obj);
                    myData.MyLevel = Levels.Archer;
                    break;
                }
                case Levels.Archer:
                {
                    Destroy(obj);
                    myData.MyLevel = Levels.Warior;
                    break;
                }
            }
        }
        else
        {
            cell = null;
            transform.position = startPos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cell"))
        {
            other.GetComponent<Cell>().OnActivate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cell"))
        {
            other.GetComponent<Cell>().OnDisactive();
        }
    }
}
