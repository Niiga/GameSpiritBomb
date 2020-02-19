using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public AudioManager audioManager;
    public HUD hud;
    // Start is called before the first frame update
    void Start()
    {
        InitTheLevel();
    }
    void InitTheLevel(){
        GameManager.instance.GetMainCamera();
        GameManager.instance.GetAudioManager(audioManager);
        GameManager.instance.GetHUD(hud);

        // GameManager.instance.GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        //重新加载当前关卡
        if(Input.GetKey(KeyCode.LeftShift)){
            if(Input.GetKeyDown(KeyCode.R)){
                GameManager.instance.RefreshLevel();
            }
        }
    }
}
