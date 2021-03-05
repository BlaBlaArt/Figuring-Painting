using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    [SerializeField] private Collider colliderToTrigger;
    
    [SerializeField] private LayerMask CellLayer;
   
    [SerializeField] private float HeightOfLevel;

    private CharacterData myData;
    
    private Cell cell;

    private Cell startCell;
    private Vector3 startPos;

    private void Start()
    {
        myData = GetComponent<CharacterData>();
        startPos = transform.position;
        startCell = null;
    }

    private void OnMouseDown()
    {
        DragController.Instance.SetObjectToDrag(gameObject);
        // dragController.SetObjectToDrag(gameObject);
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
        DragController.Instance.DisableDrag();

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
                CheckMergePosibility(cell.MyObject);
            }
            else
            {
                MoveToFreeCell();
            }
        }
        else
        {
            MoveToStart();

        }
    }

    private void MoveToFreeCell()
    {
        transform.position = cell.StartPos;
        startPos = cell.StartPos;
        if(startCell != null)
            startCell.OnObjectMove();
        startCell = cell;
        OnSucssesfullSpawn();
        cell.OnGetObject(gameObject);
    }
    
    private void CheckMergePosibility(GameObject objOnCell)
    {
        var data = objOnCell.GetComponent<CharacterData>();
        CheckLevelObjectOnCell(objOnCell, data.myCharacterClass);
    }

    private void CheckLevelObjectOnCell(GameObject objOnCell, CharacterClass characterClass)
    {
        if (myData.myCharacterClass == characterClass)
        {
            switch (characterClass)
            {
                case CharacterClass.Warior:
                {
                    OnMerge(objOnCell);
                    break;
                }
                case CharacterClass.Archer:
                {
                    OnMerge(objOnCell);
                    break;
                }
            }
        }
        else
        {
            cell = null;
            MoveToStart();
    
        }
    }

    private void OnSucssesfullSpawn()
    {
        LevelController.Instance.OnSpawnCharacter?.Invoke(myData.CharacterNum);
    }
    
    private void MoveToStart()
    {
        colliderToTrigger.isTrigger = true;
        transform.position = startPos;
        if(startCell != null)
            startCell.OnGetObject(gameObject);
        this.WaitAndDoCoroutine(0.15f, () => colliderToTrigger.isTrigger = false);  
    }

    private void OnMerge(GameObject objOnCell)
    {
        Destroy(objOnCell);
        MoveToFreeCell();
        transform.localScale += transform.localScale;
        OnSucssesfullSpawn();
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
