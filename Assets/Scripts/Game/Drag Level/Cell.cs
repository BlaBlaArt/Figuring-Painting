using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector3 StartPos;

    public GameObject MyObject;

    private BoxCollider myCollider;

    private float heigth;
    
    private void Start()
    {
        myCollider = GetComponent<BoxCollider>();
        StartPos = transform.position;
        heigth = DragController.Instance.CellsHeight;
    }

    public void OnActivate()
    {
        if(MyObject == null)
            transform.position += Vector3.up * heigth;
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
        transform.position = StartPos;
    }
}
