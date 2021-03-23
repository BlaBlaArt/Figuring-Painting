using System.Collections;
using DG.Tweening;
using TFPlay.UI;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    [SerializeField] private Collider colliderToTrigger;
    
    [SerializeField] private LayerMask CellLayer;
    private float deltaHeightOfLevel, heightToMove;
    private float offsetPosForward;

    private CharacterData myData;
    
    public Cell cell;

    public Cell startCell;
    public Vector3 startPos;

    private IAnimationController myAnimationController;

    private float startRadiusOfColider, startHeightOfColider;
    private Vector3 startCenterOfColider;
    
    private bool canCollide = true;

    public float OffsetOfSpawn;
    
    private Transform myTransform;

    public bool IsSpawnOnCell = false;
    
    private void Start()
    {
        var colider = GetComponent<CapsuleCollider>();
        startRadiusOfColider = colider.radius;
        startHeightOfColider = colider.height;
        startCenterOfColider = colider.center;
        colider.radius = 0.01f;
        colider.center = new Vector3(colider.center.x, -0.6f, colider.center.z);
        colider.height = 4.15f;
        myAnimationController = GetComponentInChildren<IAnimationController>();
        deltaHeightOfLevel = DragController.Instance.DeltaHeightOfLevel;
        offsetPosForward = DragController.Instance.OffsetToForward;
        myData = GetComponent<CharacterData>();
        startPos = transform.position;
        //startCell = null;
        myTransform = transform;
        heightToMove = myTransform.position.y + deltaHeightOfLevel;
        this.WaitAndDoCoroutine(1.75f, () => GameC.Instance.ShowFinishButton());
    }
    

    public void OnStartInvoke(float offsetOfSpawn)
    {
        OffsetOfSpawn = offsetOfSpawn;
        StartCoroutine(OnStart());
    }

    private IEnumerator OnStart()
    {
        yield return new WaitForEndOfFrame();
        myAnimationController.Run();
        var startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - OffsetOfSpawn);
        transform.DOMove(startPos, 0.75f).OnComplete(() => myAnimationController.Idle());
        this.startPos = startPos;

    }

    private void OnDisable()
    {
        var colider = GetComponent<CapsuleCollider>();
        colider.radius = startRadiusOfColider;
        colider.height = startHeightOfColider;
        colider.center = startCenterOfColider;
        transform.position = startPos;
    }

    public void OnSpawnOnCell(Cell cell)
    {
        IsSpawnOnCell = true;
        this.cell = cell;
        startCell = cell;
        this.WaitAndDoCoroutine(0.2f,() => FightController.Instance.TmpObjects.Add(gameObject));
        this.WaitAndDoCoroutine(0.1f, () => cell.OnDisactive());
    }
    
    private void OnMouseDown()
    {
        Taptic.Light();
        
        DragController.Instance.SetObjectToDrag(gameObject);
        // dragController.SetObjectToDrag(gameObject);
        if (cell != null)
        {
            cell.OnObjectMove();
            cell.OnActivate();
        }
        
        myAnimationController.OnDrag(true);
        
        Debug.Log("DOWN");
    }

    private void OnMouseDrag()
    {
        var targetPosition =  new Vector3(myTransform.position.x + offsetPosForward, heightToMove, myTransform.position.z);
        myTransform.position = targetPosition;
    }

    private void OnMouseUp()
    {
        Taptic.Light();
        DragController.Instance.DisableDrag();

        myAnimationController.OnDrag(false);
        
        CheckCell();
        
        FightController.Instance.OnDragObjectDown();
    }

    private void CheckCell()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit,50, CellLayer))
        {
            Debug.Log("Cell");
            cell = hit.transform.GetComponent<Cell>();

            if (cell.MyObject != null)
            {
                Debug.Log("MergeEnter");
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
        //transform.position = cell.StartPos;
        transform.DOMove(cell.transform.position, Vector3.Distance(transform.position, cell.transform.position)/10).SetEase(Ease.Linear);
        startPos = cell.transform.position;
        if(startCell != null)
            startCell.OnObjectMove();
        startCell = cell;
        OnSucssesfullSpawn();
        cell.OnGetObject(gameObject);
        GameC.Instance.ShowFinishButton();
    }
    
    private void CheckMergePosibility(GameObject objOnCell)
    {
        var data = objOnCell.GetComponent<CharacterData>();
        CheckLevelObjectOnCell(objOnCell, data.myCharacterClass);
    }

    private void CheckLevelObjectOnCell(GameObject objOnCell, CharacterClass characterClass)
    {
        
        //////////////////////////////////////////////////////MERGEEEEEEEEEEEEEEEEEEEEEEEEEE//
       // if (myData.myCharacterClass == characterClass)
       // {
       //     switch (characterClass)
       //     {
       //         case CharacterClass.Warior:
       //         {
       //             OnMerge(objOnCell);
       //             break;
       //         }
       //         case CharacterClass.Archer:
       //         {
       //             OnMerge(objOnCell);
       //             break;
       //         }
       //     }
       // }
       // else
        {
          //  cell = null;
            ChangeObjectOnCellAndMe();
        }
    }

    private void ChangeObjectOnCellAndMe()
    {
        Debug.Log("StartCell " + startCell);
        if (startCell != null)
        {
            var dragObject = cell.MyObject.GetComponent<DragObject>();
            
            var startpos = startPos;
            startPos = dragObject.startPos;
            dragObject.startPos = startpos;
            
            var tmpcell = startCell;
            
            startCell.OnGetObject(dragObject.gameObject);
            startCell = dragObject.startCell;

            dragObject.startCell.OnGetObject(gameObject);
            dragObject.startCell = tmpcell;
            dragObject.cell = tmpcell;
            
            
            
            dragObject.MoveToStart();
            MoveToStart();
            GameC.Instance.ShowFinishButton();
        }
        else
        {
            MoveToStart();
        }
    }

    private void OnSucssesfullSpawn()
    {
        if (!IsSpawnOnCell)
            LevelController.Instance.OnSpawnCharacter?.Invoke(myData.CharacterNum);
    }
    
    public void MoveToStart()
    {
        canCollide = false;
        transform.DOMove(startPos, Vector3.Distance(transform.position, startPos)/10).SetEase(Ease.Linear)
            .OnComplete(
                () =>
                {
                    canCollide = true;
                });
        
        // transform.position = startPos;
        if(startCell != null)
            startCell.OnGetObject(gameObject);

        if (cell != null)
        {
            cell.OnDisactive();
            cell = null;
        }
        // this.WaitAndDoCoroutine(0.15f, () => colliderToTrigger.isTrigger = false);  
    }

    private void OnMerge(GameObject objOnCell)
    {
        Destroy(objOnCell);
        MoveToFreeCell();
        myData.CharacterLevel++;
        myData.OnLevelChange();
        OnSucssesfullSpawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canCollide && other.CompareTag("Cell"))
        {
            other.GetComponent<Cell>().OnActivate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (canCollide && other.CompareTag("Cell"))
        {
            other.GetComponent<Cell>().OnDisactive();
        }
    }

}
