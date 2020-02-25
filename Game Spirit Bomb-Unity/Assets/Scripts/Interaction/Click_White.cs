using UnityEngine;

[RequireComponent(typeof(Cell))]
public class Click_White : MonoBehaviour
{
    protected Cell cell;
    // Start is called before the first frame update
    void Start()
    {
        cell = GetComponent<Cell>();
    }

    void OnMouseDown(){
        if(!GameManager.instance.WaitForInput) return;
        GameManager.instance.InteractWithWhiteCell(cell.index);
    }
}
