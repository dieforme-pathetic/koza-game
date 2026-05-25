using UnityEngine;
using System.Collections.Generic;

public class SceneAudioController : MonoBehaviour
{
    [Header("Музыка")]
    [SerializeField] private AudioClip musicClip;
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.4f;
    [SerializeField] private bool playMusicOnStart = true;
    
    [Header("Звуки шагов")]
    [SerializeField] private List<AudioClip> footstepClips;
    [Range(0f, 1f)] [SerializeField] private float footstepVolume = 0.3f;
    [SerializeField] private float footstepInterval = 0.5f; // секунд между шагами
    
    [Header("Дополнительные звуки")]
    [SerializeField] private AudioClip jumpClip;
    [Range(0f, 1f)] [SerializeField] private float jumpVolume = 0.5f;
    
    [SerializeField] private AudioClip interactionClip;
    [Range(0f, 1f)] [SerializeField] private float interactionVolume = 0.6f;
    
    // Компоненты
    private AudioSource musicSource;
    private AudioSource sfxSource;
    
    // Для шагов
    private float footstepTimer;
    private bool isMoving;
    
    void Awake()
    {
        // Создаем два независимых источника звука
        CreateAudioSources();
    }
    
    void Start()
    {
        if (playMusicOnStart && musicClip != null)
        {
            PlayMusic();
        }
    }
    
    void Update()
    {
        // Обработка шагов (если игрок движется)
        HandleFootsteps();
    }
    
    private void CreateAudioSources()
    {
        // Источник для музыки
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;
        musicSource.spatialBlend = 0f; // 2D звук
        
        // Источник для звуковых эффектов
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = footstepVolume;
        sfxSource.spatialBlend = 0f;
    }
    
    // ========== Публичные методы для управления музыкой ==========
    
    public void PlayMusic()
    {
        if (musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.Play();
        }
    }
    
    public void StopMusic()
    {
        musicSource.Stop();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }
    
    // ========== Звуки шагов ==========
    
    public void StartMoving()
    {
        isMoving = true;
        footstepTimer = 0f;
    }
    
    public void StopMoving()
    {
        isMoving = false;
        footstepTimer = 0f;
    }
    
    public void PlayFootstep()
    {
        if (footstepClips != null && footstepClips.Count > 0)
        {
            int randomIndex = Random.Range(0, footstepClips.Count);
            sfxSource.PlayOneShot(footstepClips[randomIndex], footstepVolume);
        }
    }
    
    private void HandleFootsteps()
    {
        if (isMoving)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                PlayFootstep();
                footstepTimer = 0f;
            }
        }
    }
    
    // ========== Звуки действий ==========
    
    public void PlayJump()
    {
        if (jumpClip != null)
        {
            sfxSource.PlayOneShot(jumpClip, jumpVolume);
        }
    }
    
    public void PlayInteraction()
    {
        if (interactionClip != null)
        {
            sfxSource.PlayOneShot(interactionClip, interactionVolume);
        }
    }
    
    // Перегруженный метод для любых дополнительных звуков
    public void PlaySound(AudioClip clip, float volume = 0.5f)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }
}
