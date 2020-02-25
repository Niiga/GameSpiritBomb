using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header ("Prefabs")]
    public GameObject GridPrefab;       //网格 白
    public GameObject VirusRedPrefab;   //敌方主将 红
    public GameObject VirusGreyPrefab;  //敌方人员 灰
    public GameObject PlayerGreenPrefab;//我方主将 绿
    [Header ("Grid")]
    public float margin;
    public float cellMoveTime = .2f;
    [Header ("Level")]
    public int Level = 1;//关卡数
    public int TotalGrids { get { return (Level*2) * (Level*3); } }//总网格数
    public int RowCount { get { return Level*2; } }//每一排网格数
    public int ColumnCount{ get { return Level*3; }}//每一纵队网格数
    public int GreenCount { get; protected set; }//总体绿色格子的数目
    public int HealthCount { get{return GreenCount;}}
    public int virusCount { get; protected set; }//病毒个数
    public int Steps { get; protected set; }//步骤数
    public float perUnitScale { get; protected set; }//每一格的单位大小
    public Camera cam { get; protected set; }//获取主摄像机
    public Cell VirusRed { get; protected set; }
    public SpawnGreyVirus VirusSpawner { get; protected set; }
    public Cell[] GreenCells { get; protected set; }
    public List<Cell> GreyCells { get; protected set; }
    public GameObject GridRoot { get; protected set; }
    public AudioManager audioManager {get; protected set; }
    public bool WaitForInput{get; protected set;}//等待输入，真为等待输入，否为不接受输入
    public HUD hud{get; protected set; }
    public static GameManager instance;
    protected Cell ClickedGreyCell;
    protected bool FirstStep = true;

    //设置GameManager为Singleton，且不会因为加载关卡被重置
    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad (gameObject);
        }
        else if (instance != null) {
            Destroy (gameObject);
        }
    }

    //获取主摄像机
    public void GetMainCamera() {
        cam = Camera.main;
    }

    //获取AudioManager
    public void GetAudioManager(AudioManager _audioManager){
        audioManager = _audioManager;
    }

    //获取HUD
    public void GetHUD(HUD _hud){
        hud = _hud;
    }

    //卸载所有关卡资源
    public void UnloadLevel() {
        GreenCount = 0;
        FirstStep  = true;
        GreyCells.Clear();
        Destroy(GridRoot.gameObject);
    }

    //重新加载关卡
    public void RefreshLevel() {
        WaitForInput = true;
        UnloadLevel();
        GenerateGrid();
    }

    //加载下一关
    public void NextLevel() {
        Level++;
        RefreshLevel ();
    }

    //加载上一关
    public void PreviousLevel() {
        Level--;
        if (Level < 1)
            Level = 1;
        RefreshLevel ();
    }

    //创建格子
    public void GenerateGrid() {
        //创建一个网格根节点
        GridRoot = new GameObject ("GridRoot");
        GridRoot.transform.position = Vector2.zero;

        //根据摄像机的scale，以及网格总数，获取单元网格的大小
        Camera cam = Camera.main;
        perUnitScale = (cam.orthographicSize * 2 * cam.aspect-margin) / RowCount;

        //设置网格根节点的起始点
        Vector2 InitOffset = new Vector2 (-perUnitScale * (RowCount - 1) / 2f, -perUnitScale * (ColumnCount - 1) / 2f);
        GridRoot.transform.position = InitOffset;

        //生成网格与所有绿色格子，并关掉所有的绿色格子
        GreenCells = new Cell[TotalGrids];
        Vector2 pos = Vector2.zero;
        for (int i = 0; i < TotalGrids; i++) {
            pos.y = i / RowCount * perUnitScale;
            pos.x = i % RowCount * perUnitScale;

            GameObject grid = Instantiate (GridPrefab);
            grid.transform.parent = GridRoot.transform;
            grid.transform.localPosition = pos;
            grid.transform.localPosition += Vector3.forward;
            grid.transform.localScale *= perUnitScale;
            grid.GetComponent<Cell> ().SetIndex (i % RowCount, i / RowCount);

            GreenCells[i] = Instantiate (PlayerGreenPrefab).GetComponent<Cell> ();
            GreenCells[i].transform.parent = GridRoot.transform;
            GreenCells[i].transform.localPosition = pos;
            GreenCells[i].transform.localScale *= perUnitScale;
            GreenCells[i].SetIndex (i % RowCount, i / RowCount);
            GreenCells[i].DisableCell ();
        }

        //放置红色格子
        Vector2Int index_red = new Vector2Int (Random.Range (0, RowCount), Random.Range (0, RowCount));
        VirusRed = Instantiate (VirusRedPrefab).GetComponent<Cell> ();
        VirusRed.transform.parent = GridRoot.transform;
        VirusRed.transform.localPosition = new Vector2 (index_red.x * perUnitScale, index_red.y * perUnitScale);
        VirusRed.transform.localScale *= perUnitScale;
        VirusRed.SetIndex (index_red);

        VirusSpawner = VirusRed.GetComponent<SpawnGreyVirus> ();

        GreyCells = new List<Cell> ();
        virusCount = 1;
        Steps = 0;
        WaitForInput = true;
    }

    //放置灰色格子
    public void PlaceGreyCell(Vector2Int pos) {
        GameObject grid = Instantiate (VirusGreyPrefab);
        grid.transform.parent = GridRoot.transform;
        grid.transform.localPosition = perUnitScale * (Vector2)pos;
        grid.transform.localScale *= perUnitScale;
        grid.GetComponent<Cell> ().SetIndex (pos.x, pos.y);
        GreyCells.Add (grid.GetComponent<Cell> ());
        virusCount ++;
    }

//用coroutine处理格子的交互
//Coroutine开始
    IEnumerator Coroutine_WhiteCellInteraction(Vector2Int pos){
        WaitForInput = false;
        if (FirstStep)
            FirstStep = false;
        Steps++;
        GreenCount++;
        audioManager.PlaySound(2);
        getGreenCell (pos).EnableCell ();
        UpdateWholeGrid ();
        //停顿让移动完全进行
        yield return new WaitForSeconds(cellMoveTime+.1f);

        //如果游戏未结束，重新开启输入检测
        if(!CheckEndState()) WaitForInput = true;

        yield return null;
    }
    IEnumerator Coroutine_GreyCellInteraction(Cell cell){
        WaitForInput = false;
        if (FirstStep)
            FirstStep = false;
        Steps++;
        ReduceInfectionLevel(cell);
        ClickedGreyCell = cell;
        UpdateWholeGrid ();
        ClickedGreyCell = null;
        //停顿让移动完全进行
        yield return new WaitForSeconds(cellMoveTime);

        //如果游戏未结束，重新开启输入检测
        if(!CheckEndState()) WaitForInput = true;

        yield return null;
    }
    IEnumerator Coroutine_RedCellInteraction(Vector2Int pos){
        WaitForInput = false;
        Steps++;
        UpdateGreen ();
        UpdateVirusGrey ();
        UpdateGreen ();
        //停顿让移动完全进行
        yield return new WaitForSeconds(cellMoveTime);

        //如果游戏未结束，重新开启输入检测
        if(!CheckEndState()) WaitForInput = true;

        yield return null;
    }
    IEnumerator Coroutine_MovePieces(Vector2Int pos, Cell cell, float time = 1){
        Vector3 initPos = cell.transform.localPosition;
        Vector3 targetPos = perUnitScale*(Vector2)pos;
        for(float timer = 0; timer < 1; timer += Time.deltaTime/time){
            cell.transform.localPosition = Vector3.Lerp(initPos, targetPos, timer);
            yield return null;
        }
        cell.transform.localPosition = perUnitScale*(Vector2)pos;
        yield return null;
    }
//Coroutine结束

    //点击空白格子，放置一个绿色格子
    public void InteractWithWhiteCell(Vector2Int pos) {
        StartCoroutine(Coroutine_WhiteCellInteraction(pos));
    }
    //点击灰色格子，并削弱它
    public void InteractWithGreyCell(Cell cell){
        StartCoroutine(Coroutine_GreyCellInteraction(cell));
    }
    //与红色格子交互，并限制它的移动
    public void InteractWithRedCell(Vector2Int pos){
        StartCoroutine(Coroutine_RedCellInteraction(pos));
    }
    //移动格子
    public void MovePieces(Vector2Int pos, Cell cell, float time = 1){
        StartCoroutine(Coroutine_MovePieces(pos, cell, time));
    }

    //检查胜败条件
    protected bool CheckEndState(){
        if(CheckFillState()){
            if(GreenCount > virusCount){
                hud.DisplayWinScenes();
            }
            else{
                hud.DisplayLoseScenes();
            }
            return true;
        }
        else{
            if(CheckLoseState()){
                WaitForInput = false;
                hud.DisplayLoseScenes();
                return true;
            }
            else if(CheckWinState()){
                WaitForInput = false;
                hud.DisplayWinScenes();
                return true;
            }
        }

        return false;
    }

    //遭遇绿色格子
    protected bool ResolveGreenCell(Cell greenCell, Cell greyCell){
        int infectionLevel = greyCell.GetComponent<InfectionLevel>().level;
        int healthLevel    = greenCell.GetComponent<HealthLevel>().level;

        if(infectionLevel >= healthLevel){
            return true;
        }
        else{
            return false;
        }
    }

    //消灭灰色格子
    public void RemoveGreyCell(Cell cell){
        virusCount --;
        audioManager.PlaySound(3);
        GreyCells.Remove(cell);
        Destroy(cell.gameObject);
    }

    //消灭绿色格子
    protected void RemoveGreenCell(Vector2Int pos) {
        GreenCount--;
        audioManager.PlaySound(4);
        getGreenCell (pos).DisableCell ();
    }

    //削弱灰色格子的感染值
    protected void ReduceInfectionLevel(Cell cell){
        cell.GetComponent<InfectionLevel>().ReduceLevel();
    }

    //更新整个网格
    protected void UpdateWholeGrid() {
        UpdateGreen ();
        UpdateVirusGrey ();
        UpdateVirusRed ();
        UpdateGreen ();
        // Debug.Log ($"3 更新整个网格");
    }

    //更新所有绿色格子
    protected void UpdateGreen() {
        for(int i=0; i<GreenCells.Length; i++){
            if(GreenCells[i].IF_Activate){
                GreenCells[i].GetComponent<HealthLevel>().UpdateLevel();
            }
        }
    }

    //更新红色格子的位置
    protected void UpdateVirusRed() {
        VirusRed.PrepareStep ();

        if (IfPosGreen (VirusRed.nextIndex) || IfPosGrey (VirusRed.nextIndex)) {
            VirusRed.Reset ();
        }
        else {
            VirusSpawner.SpawnGreyCellOnCurrentPos ();
            VirusRed.Step ();
        }
    }

    //更新灰色格子
    protected void UpdateVirusGrey() {
        for (int i=GreyCells.Count-1; i>=0; i--) {
            //跳过被点击的灰色格子
            if(GreyCells[i] == ClickedGreyCell) continue;
            //更新灰色格子的位置
            GreyCells[i].PrepareStep ();

            if (IfPosGrey (GreyCells[i].nextIndex) || IfPosRed (GreyCells[i].nextIndex)) {
                GreyCells[i].Reset ();
            }
            else if (IfPosGreen (GreyCells[i].nextIndex)) {
                if(ResolveGreenCell(getGreenCell(GreyCells[i].nextIndex), GreyCells[i])){
                    RemoveGreenCell(GreyCells[i].nextIndex);
                    PlaceGreyCell(GreyCells[i].index);
                    GreyCells[i].Step ();
                    continue;
                }
                else{
                    RemoveGreyCell(GreyCells[i]);
                    continue;
                }
            }
            else {
                GreyCells[i].Step ();
            }

            //移动之后，灰色格子感染值+1
            GreyCells[i].GetComponent<InfectionLevel>().UpgradeLevel();
        }
    }

    //检查给定位置是否有网格
    public bool CheckGrid(Vector2Int pos) {
        if (pos.x < 0 || pos.x >= RowCount || pos.y < 0 || pos.y >= ColumnCount)
            return false;
        return true;
    }

    //检查当前网格是否为绿色
    public bool IfPosGreen(Vector2Int pos) {
        if(!CheckGrid(pos)) return false;
        return getGreenCell (pos).IF_Activate;
    }

    //检查当前网格是否为红色
    public bool IfPosRed(Vector2Int pos) {
        if(!CheckGrid(pos)) return false;
        return VirusRed.index == pos;
    }

    //检查当前网格是否为灰色
    public bool IfPosGrey(Vector2Int pos) {
        if(!CheckGrid(pos)) return false;
        for(int i=0; i<GreyCells.Count; i++){
            if (GreyCells[i].index == pos)
                return true;
        }

        return false;
    }

    //获取给定位置的绿色网格
    protected Cell getGreenCell(Vector2Int pos) {
        int index = pos.y * RowCount + pos.x;
        return GreenCells[index].GetComponent<Cell> ();
    }

    //检查网格是否占满
    protected bool CheckFillState(){
        for(int i=0; i<TotalGrids; i++){
            Vector2Int pos = new Vector2Int(i%RowCount, i/RowCount);
            if(!IfPosGrey(pos)){
                if(!IfPosGreen(pos)){
                    if(!IfPosRed(pos)){
                        return false;
                    }
                }
            }
        }

        return true;
    }

    //检查胜利条件
    protected bool CheckWinState() {
        //先检查角落，只对格子内的区域做检查
        List<Vector2Int> possibleSteps = new List<Vector2Int> ();
        Vector2Int right, up, left, down;
        right = new Vector2Int (1, 0);
        up = new Vector2Int (0, 1);
        left = new Vector2Int (-1, 0);
        down = new Vector2Int (0, -1);

        if (CheckGrid (VirusRed.index + right))
            possibleSteps.Add (right);
        if (CheckGrid (VirusRed.index + up))
            possibleSteps.Add (up);
        if (CheckGrid (VirusRed.index + left))
            possibleSteps.Add (left);
        if (CheckGrid (VirusRed.index + down))
            possibleSteps.Add (down);

        //检查周围的区域是否都有绿色格子，如果都有为真，否则为假。
        for (int i = 0; i < possibleSteps.Count; i++) {
            if (!IfPosGreen (VirusRed.index + possibleSteps[i]))
                return false;
        }

        return true;
    }

    //检查失败条件
    protected bool CheckLoseState() {
        return GreenCount == 0 && !FirstStep;
    }
}
