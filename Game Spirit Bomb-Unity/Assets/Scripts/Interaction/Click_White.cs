using System.Collections;
using System.Collections.Generic;
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
        GameManager.instance.InteractWithWhiteCell(cell.index);
    }
}
