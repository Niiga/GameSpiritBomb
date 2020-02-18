using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpawnGreyVirus))]
public class SpawnGreyVirus : MonoBehaviour
{
    Cell cell;
    void Start()
    {
        cell = GetComponent<Cell>();
    }
    public void SpawnGreyCellOnCurrentPos(){
        GameManager.instance.PlaceGreyCell(cell.index);
    }
}
