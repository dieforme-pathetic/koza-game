using UnityEngine;

public class Lamp : MonoBehaviour
{
    public Light lampLight;
    public SpriteRenderer lampSprite;
    public Color onColor = Color.yellow;
    public Color offColor = Color.gray;
    
    private bool isOn = false;
    
    void Start()
    {
        if (lampLight != null)
            lampLight.enabled = false;
        
        if (lampSprite != null)
            lampSprite.color = offColor;
    }
    
    public void TurnOn()
    {
        if (isOn) return;
        
        isOn = true;
        
        if (lampLight != null)
            lampLight.enabled = true;
        
        if (lampSprite != null)
            lampSprite.color = onColor;
    }
}