using DG.Tweening;
using UnityEngine;

public class Cell : MonoBehaviour
{
   // public Vector3 StartPos;

    public GameObject MyObject;

    public int Id;
    
    private BoxCollider myCollider;

    private Material myMaterial;

    private Color startCellColor, activeCellColor;
    
   // private float heigth;
    
    private void Start()
    {
        myCollider = GetComponent<BoxCollider>();
      //  StartPos = transform.position;
        var dragC = DragController.Instance;
     //   heigth = dragC.CellsHeight;
        startCellColor = dragC.startCellColor;
        activeCellColor = dragC.activeCellColor;
        
        SetMaterial();
    }

    private void SetMaterial()
    {
        var tmpMat = GetComponent<Renderer>().material;
        myMaterial = new Material(tmpMat.shader);
        myMaterial.color = startCellColor;
        GetComponent<Renderer>().material = myMaterial;
    }

    public void OnActivate()
    {
     //  if(MyObject == null)
            //transform.position += Vector3.up * heigth;

            myMaterial.DOColor(activeCellColor, 0.25f);
    }

    public void OnGetObject(GameObject Obj)
    {
        OnDisactive();
        MyObject = Obj;
    }

    public void OnObjectMove()
    {
        MyObject = null;
      //  OnActivate();
    }
    
    public void OnDisactive()
    {
       // transform.position = StartPos;
       myMaterial.DOColor(startCellColor, 0.25f);
    }
}
