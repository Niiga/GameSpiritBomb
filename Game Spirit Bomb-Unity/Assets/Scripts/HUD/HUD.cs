using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // 按钮 
    public Button startButton;          // 开始按钮
    public Button runingBackButton;     // 运行中的返回按钮
    public Button gameoverWinButton;    // 游戏结束界面按钮
    public Button gameoverLoseButton;   // 游戏结束界面按钮
    public Button ExitButton;           // 开始界面退出按钮
    public Button ExitOKButton;     // 退出界面确定按钮
    public Button ExitCancelButton; // 退出界面取消按钮

    public Button staffButton;          // 制作组名单按钮
    public GameObject staffOBJ;         // 制作组名单界面
    public Image gameoverImage;         // 游戏结束界面图片
    public ActivrGoDown goDownScript;   // 游戏结束界面动态图片控制代码

    public Button OFFhowToPlayButton;  // 打开 玩法介绍按钮
    public Button ONhowToPlayButton;   // 关闭 玩法介绍按钮
    public GameObject howToPlayOBJ;   // 关闭 玩法介绍按钮


    float staffCD = 5f;
    float staffTime = 0f;
    bool isSetStaff = false;
    // 界面
    public Transform Running;

    // 声音
    public GameObject audioOBJ;
    // 显示 Text
    public Text levelText;
    public Text stepText;
    public Text virusCountText;
    public Text healthCountText;

    public Sprite WinImg;
    public Sprite LoseImg;

    private void Awake() {
        OnBackButton ();

    }
    private void Start() {
        // 开始游戏 按钮
        startButton.onClick.AddListener (StartGame);
        // 运行界面的返回按钮
        runingBackButton.onClick.AddListener (OnBackButton);
        // 显示制作组名单按钮
        staffButton.onClick.AddListener (DisplayStaff);
        // 游戏结束 胜利界面按钮
        gameoverWinButton.onClick.AddListener (NextLevel);
        // 游戏结束 失败界面按钮
        gameoverLoseButton.onClick.AddListener (RefreshLevel);
        // 显示游戏玩法按钮
        OFFhowToPlayButton.onClick.AddListener (DispalyHowToPlay);
        // 关闭游戏玩法按钮
        ONhowToPlayButton.onClick.AddListener (DispalyHowToPlay);
        // 打开退出界面按钮
        ExitButton.onClick.AddListener (DisplayQuitOrCancelScene);
        // 退出游戏按钮
        ExitOKButton.onClick.AddListener (QuitSceneOKButton);
        // 取消退出游戏
        ExitCancelButton.onClick.AddListener (DisplayQuitOrCancelScene);
    }

    private void Update() {
        // 界面文字显示
        levelText.text = "level:" + GameManager.instance.Level;
        healthCountText.text = "HealthCount:" + GameManager.instance.HealthCount;
        virusCountText.text = "VirusCount:" + GameManager.instance.virusCount;
        stepText.text = "Step:" + GameManager.instance.Steps;

        // 制作组名单自动关闭
        if (Time.time - staffCD > staffTime && isSetStaff == true) {
            staffOBJ.SetActive (false);
            isSetStaff = false;
        }

    }

    // 按 退出按钮
    void DisplayQuitOrCancelScene() {
        ExitOKButton.transform.parent.gameObject.SetActive (!ExitOKButton.transform.parent.gameObject.activeSelf);
    }

    void QuitSceneOKButton() {
        Application.Quit ();
    }

    // 按 显示or关闭 游戏玩法介绍按钮
    void DispalyHowToPlay() {
        Debug.Log ($"{howToPlayOBJ.activeSelf}");
        howToPlayOBJ.SetActive (!howToPlayOBJ.activeSelf); // 默认关闭
    }

    // 按 返回按钮
    void OnBackButton() {
        Running.gameObject.SetActive (false);
        startButton.transform.parent.gameObject.SetActive (true);
        gameoverLoseButton.transform.parent.gameObject.SetActive (false);
        audioOBJ.GetComponent<AudioManager> ().PlayAudio (0);
    }
    // 按 开始按钮
    void StartGame() {
        startButton.transform.parent.gameObject.SetActive (false);
        Running.gameObject.SetActive (true);
        audioOBJ.GetComponent<AudioManager> ().PlayAudio (1);
    }

    // 胜利界面
    public void DisplayWinScenes() {
        gameoverImage.transform.parent.gameObject.SetActive (true);
        gameoverImage.sprite = WinImg;
        goDownScript.isTime = true;
        goDownScript.ChangeImage (goDownScript.WinImage);
        audioOBJ.GetComponent<AudioManager> ().PlaySound (6); // 音效 success
        StartCoroutine ("IWinDisplay");
    }
    // 失败界面
    public void DisplayLoseScenes() {
        gameoverImage.transform.parent.gameObject.SetActive (true);
        gameoverImage.sprite = LoseImg;
        goDownScript.isTime = true;
        goDownScript.ChangeImage (goDownScript.LoseImage);
        audioOBJ.GetComponent<AudioManager> ().PlaySound (7); // 音效 fail
        StartCoroutine ("ILoseDisplay");
    }
    // 延迟显示胜利界面按钮
    IEnumerator IWinDisplay() {
        yield return 0.1f;
        gameoverWinButton.gameObject.SetActive (true);
    }
    // 延迟显示失败界面按钮
    IEnumerator ILoseDisplay() {
        yield return 0.1f;
        gameoverLoseButton.gameObject.SetActive (true);
    }

    // 进行下一关
    void NextLevel() {
        gameoverImage.transform.parent.gameObject.SetActive (false);
        gameoverWinButton.gameObject.SetActive (false);
        gameoverLoseButton.gameObject.SetActive (false);
        GameManager.instance.NextLevel ();
        goDownScript.ResetActivr ();
    }

    // 重玩本关
    void RefreshLevel() {
        gameoverWinButton.gameObject.SetActive (false);
        gameoverLoseButton.gameObject.SetActive (false);
        gameoverImage.transform.parent.gameObject.SetActive (false);
        GameManager.instance.RefreshLevel ();
        goDownScript.ResetActivr ();
    }

    // 按 显示制作组名单按钮
    void DisplayStaff() {
        staffTime = Time.time;
        isSetStaff = true;
        staffOBJ.SetActive (true);
    }
}
