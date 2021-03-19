using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject[] Cells;
    public CellController[,] trueCells = new CellController[3, 3];

    private void Start()
    {
        
        
        for (int i = 0; i < 3; i++)
        {
            trueCells[0, i] = Cells[i].GetComponent<CellController>();
            Cells[i].GetComponent<CellController>().id = i;
        }

        for (int i = 3, j =0; i < 6; i++, j++)
        {
            trueCells[1, j] = Cells[i].GetComponent<CellController>();
            Cells[i].GetComponent<CellController>().id = i;
        }
        
        for (int i = 6, j =0; i < 9; i++, j++)
        {
            trueCells[2, j] = Cells[i].GetComponent<CellController>();
            Cells[i].GetComponent<CellController>().id = i;
        }

    }

    public void OnTurn()
    {
        
    }

    private void CheckCells()
    {
        
    }
}
