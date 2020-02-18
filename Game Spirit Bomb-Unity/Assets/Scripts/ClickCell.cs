using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Cell))]
public class ClickCell : MonoBehaviour
{
    protected Cell cell;
    // Start is called before the first frame update
    void Start()
    {
        cell = GetComponent<Cell>();
    }

    void OnMouseDown(){
        GameManager.instance.PlaceGreenCell(cell.index);
    }
}
