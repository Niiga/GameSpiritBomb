using UnityEngine;

[RequireComponent(typeof(Cell))]
public class Click_Red : MonoBehaviour
{
    protected Cell cell;
    // Start is called before the first frame update
    void Start()
    {
        cell = GetComponent<Cell>();
    }

    void OnMouseDown(){
        GameManager.instance.InteractWithRedCell(cell.index);
    }
}
