using UnityEngine;

public class CellController : MonoBehaviour
{
    public GameObject XPref, OPref;

    public int id;
    
    public bool IsX = false;
    
    public GameObject currenSymb;
    
    private void OnMouseDown()
    {
        if (IsX)
        {
            if(currenSymb != null) 
                Destroy(currenSymb);
            
            var c = Instantiate(OPref);
            c.transform.position = transform.position;
            currenSymb = c;
        }
        else
        {
            if(currenSymb != null)
                Destroy(currenSymb);

            var c = Instantiate(XPref);
            c.transform.position = transform.position;
            currenSymb = c;
        }

        IsX = !IsX;
    }
}
