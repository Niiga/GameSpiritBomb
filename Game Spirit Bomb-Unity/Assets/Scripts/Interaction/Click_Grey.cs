using UnityEngine;

[RequireComponent(typeof(Cell))]
public class Click_Grey : MonoBehaviour
{
    protected Cell cell;
    // Start is called before the first frame update
    void Start()
    {
        cell = GetComponent<Cell>();
    }

    void OnMouseDown(){
        if(!GameManager.instance.WaitForInput) return;
        GameManager.instance.InteractWithGreyCell(cell);
    }
}
