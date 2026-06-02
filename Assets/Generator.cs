using UnityEngine;
using UnityEngine.Rendering.Universal; // Добавь для Light2D

public class Generator : MonoBehaviour
{
    [Header("Лампы (2D Light)")]
    public Light2D[] lampsToActivate;  // Light2D, а не Light!
    public float targetIntensity = 10f;  // Целевая яркость
    public float offIntensity = 0f;      // Яркость когда выключена
    
    [Header("Эффекты")]
    public ParticleSystem activationEffect;
    public AudioClip activationSound;
    
    private bool isActivated = false;
    
    void Start()
    {
        // При старте выключаем все лампы
        foreach (Light2D lamp in lampsToActivate)
        {
            if (lamp != null)
                lamp.intensity = offIntensity;
        }
    }
    
    public void Activate()
    {
        if (isActivated) return;
        
        isActivated = true;
        Debug.Log("💡 ГЕНЕРАТОР АКТИВИРОВАН! Лампы загораются.");
        
        if (activationEffect != null)
            activationEffect.Play();
        
        if (activationSound != null)
            AudioSource.PlayClipAtPoint(activationSound, transform.position);
        
        // Включаем лампы (увеличиваем интенсивность)
        foreach (Light2D lamp in lampsToActivate)
        {
            if (lamp != null)
            {
                lamp.intensity = targetIntensity;
                Debug.Log($"✅ Лампа включена, интенсивность: {targetIntensity}");
            }
        }
    }
}