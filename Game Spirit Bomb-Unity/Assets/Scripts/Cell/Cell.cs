using System.Collections.Generic;
using UnityEngine;

//单位网格
public class Cell : MonoBehaviour
{
    public bool IF_Activate{get{return this.gameObject.activeSelf;}}
    public Vector2Int index{get; protected set;}
    public Vector2Int nextIndex{get; protected set;}

    //更新坐标
    public void SetIndex(Vector2Int _index){
        index = _index;
    }
    public void SetIndex(int x, int y){
        SetIndex(new Vector2Int(x,y));
    }

    //准备可走的坐标
    public virtual void PrepareStep(){
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

    //如果无法前进，则执行这一步
    public void Reset(){}

    //向指定方向前进，并更新坐标
    public void Step(){
        index = nextIndex;
        GameManager.instance.MovePieces(index, this, GameManager.instance.cellMoveTime);
    }

    //开启与关闭当前的格子
    public void EnableCell(){
        this.gameObject.SetActive(true);
    }
    public void DisableCell(){
        this.gameObject.SetActive(false);
    }
}
