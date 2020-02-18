using UnityEngine;

public class InfectionLevel : MonoBehaviour
{
    public int level{get; protected set;}
    public SpriteRenderer spriteRenderer;
    public static int MAX_LEVEL = 5;
    // Start is called before the first frame update
    void Start()
    {
        level = 1;
    }
    public void UpgradeLevel(){
        level ++;
        if(level > MAX_LEVEL) level = MAX_LEVEL;

        float levelColor = .5f-level*.5f/5;
        spriteRenderer.color = new Color(levelColor,levelColor,levelColor,1);
    }
}
