using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover_Grid : MonoBehaviour
{
    public Color HighLightColor = Color.white;
    public TextMesh text;
    protected Color originalColor;
    void Start(){
        originalColor = GetComponent<SpriteRenderer>().color;
    }
    void OnMouseEnter(){
    }
    void OnMouseExit(){
    }
}
