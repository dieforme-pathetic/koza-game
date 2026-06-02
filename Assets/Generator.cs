using UnityEngine;

public class Generator : MonoBehaviour
{
    [Header("Лампы")]
    public Light[] lampsToActivate;
    
    [Header("Эффекты")]
    public ParticleSystem activationEffect;
    public AudioClip activationSound;
    
    private bool isActivated = false;
    
    public void Activate()
    {
        if (isActivated) return;
        
        isActivated = true;
        Debug.Log("💡 ГЕНЕРАТОР АКТИВИРОВАН! Лампы загораются.");
        
        if (activationEffect != null)
            activationEffect.Play();
        
        if (activationSound != null)
            AudioSource.PlayClipAtPoint(activationSound, transform.position);
        
        foreach (Light lamp in lampsToActivate)
        {
            if (lamp != null)
                lamp.enabled = true;
        }
    }
}