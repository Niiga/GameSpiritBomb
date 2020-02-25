using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivrGoDown : MonoBehaviour
{
    public Sprite WinImage;
    public Sprite LoseImage;
    public HUD hud;
    RectTransform hudRect;
    RectTransform rectTransform;
    bool isActivr = false;
    public bool isTime = false;
    float g = 6f;
    float curSpeed = 2f;
    float time;
    int[] jumpHeights;
    int num = 0;
    // Update is called once per frame
    private void Awake() {
        hudRect = hud.gameObject.GetComponent<RectTransform> ();
        rectTransform = GetComponent<RectTransform> ();
    }

    private void Start() {
        jumpHeights = new int[] { 
            (int)(hud.gameoverImage.rectTransform.rect.height * 2f),
            (int)(hud.gameoverImage.rectTransform.rect.height * 1.5f), 
            (int)hud.gameoverImage.rectTransform.rect.height};
    }
    bool isDown = true;
    bool isUp = false;
    void Update()
    {
        if (isTime) {
            isTime = false;
            time = Time.time;
            isActivr = true;
        }
        if (isActivr) {
            rectTransform.localPosition = new Vector3 (rectTransform.localPosition.x,
                rectTransform.localPosition.y - g * (Time.time - time) * curSpeed );
            if (rectTransform.localPosition.y <= hud.gameoverImage.rectTransform.rect.height && isDown) {
                Debug.Log ($"调用 y < image.height");
                g = -(Mathf.Abs (g) - 1.6f);
                //time = Time.time;
                isDown = false;
                isUp = true;
            }
            if (rectTransform.localPosition.y >= (float)jumpHeights[num] && isUp) {
                Debug.Log ($"调用 jumpHeights[]");
                g = (Mathf.Abs (g) - 0.6f);
                num++;
                if (num == jumpHeights.Length) {
                    isActivr = false;
                }
                isUp = false;
                isDown = true;
            }
            
        }
    }

    public void ChangeImage(Sprite _img) {
        this.GetComponent<Image> ().sprite = _img;
    }
    // 重置关卡跳跃
    public void ResetActivr() {
        
        isDown = true;
        isUp = false;
        isTime = false;
        isActivr = false;
        g = 6f;
        num = 0;
        time = Time.time;
        rectTransform.localPosition = new Vector3 (rectTransform.localPosition.x,
            hudRect.rect.height);
    }
}
