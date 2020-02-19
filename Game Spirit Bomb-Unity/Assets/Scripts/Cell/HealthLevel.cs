using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Cell))]
public class HealthLevel : MonoBehaviour
{
    public int level{get; protected set;}
    public TextMesh text;
    Cell cell;
    public void UpdateLevel(){
        if(!cell) cell = GetComponent<Cell>();
        level = 1;
        Vector2Int right, up, left, down;
        right = new Vector2Int(1,0);
        up    = new Vector2Int(0,1);
        left  = new Vector2Int(-1,0);
        down  = new Vector2Int(0,-1);


        if(GameManager.instance.IfPosGreen(cell.index + right)){level ++;}
        if(GameManager.instance.IfPosGreen(cell.index + up))   {level ++;}
        if(GameManager.instance.IfPosGreen(cell.index + left)) {level ++;}
        if(GameManager.instance.IfPosGreen(cell.index + down)) {level ++;}

        text.text = level.ToString();
    }
}
