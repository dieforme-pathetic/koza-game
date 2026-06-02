using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Lamp2D : MonoBehaviour
{
    public Light2D lampLight;
    public float targetIntensity = 10f;
    public float offIntensity = 0f;
    
    private bool isOn = false;
    
    void Start()
    {
        if (lampLight == null)
            lampLight = GetComponent<Light2D>();
        
        if (lampLight != null)
            lampLight.intensity = offIntensity;
    }
    
    public void TurnOn()
    {
        if (isOn) return;
        
        isOn = true;
        
        if (lampLight != null)
            lampLight.intensity = targetIntensity;
        
        Debug.Log($"💡 Лампа включена! Интенсивность: {targetIntensity}");
    }
    
    public void TurnOff()
    {
        if (!isOn) return;
        
        isOn = false;
        
        if (lampLight != null)
            lampLight.intensity = offIntensity;
    }
    
    public void SetIntensity(float intensity)
    {
        if (lampLight != null)
            lampLight.intensity = intensity;
    }
}