using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector3 StartPos;

    public GameObject MyObject;

    private BoxCollider myCollider;
    
    private void Start()
    {
        myCollider = GetComponent<BoxCollider>();
        StartPos = transform.position;
    }

    public void OnActivate()
    {
        transform.position += Vector3.up * 0.5f;
    }

    public void OnGetObject(GameObject Obj)
    {
        OnDisactive();
        MyObject = Obj;
    }

    public void OnObjectMove()
    {
        MyObject = null;
    }
    
    public void OnDisactive()
    {
        transform.position = StartPos;
    }
}
