using System.Collections.Generic;
using UnityEngine;

public class Cell_Red : Cell
{
    public override void PrepareStep(){
        List<Vector2Int> possibleSteps = new List<Vector2Int>();
        Vector2Int right, up, left, down;
        right = new Vector2Int(1,0);
        up    = new Vector2Int(0,1);
        left  = new Vector2Int(-1,0);
        down  = new Vector2Int(0,-1);

        if(GameManager.instance.CheckGrid(index + right)) possibleSteps.Add(right);
        if(GameManager.instance.CheckGrid(index + up))    possibleSteps.Add(up);
        if(GameManager.instance.CheckGrid(index + left))  possibleSteps.Add(left);
        if(GameManager.instance.CheckGrid(index + down))  possibleSteps.Add(down);
        
        nextIndex = index + possibleSteps[Random.Range(0, possibleSteps.Count)];       
    }
}
