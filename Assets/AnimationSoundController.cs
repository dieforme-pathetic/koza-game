using UnityEngine;

public class AnimationSoundController : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        // Находим компонент звука на этом объекте
        audioSource = GetComponent<AudioSource>();
    }

    // Эту функцию мы будем вызывать прямо из окна анимации
    public void PlayAnimationSound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            // Воспроизводим звук один раз (не прерывая другие звуки, если они наложатся)
            audioSource.PlayOneShot(clip);
        }
    }
}
