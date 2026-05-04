using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        // Устанавливаем значение слайдера из микшера при старте, если нужно
        // Или загружаем сохраненное значение через PlayerPrefs
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);
    }



    public void SetVolume(float sliderValue)
    {
        Debug.Log("Слайдер выдает: " + sliderValue); // Добавьте это
        float dB = Mathf.Log10(sliderValue) * 20;
        mainMixer.SetFloat("MasterVolume", dB);
    }
}

