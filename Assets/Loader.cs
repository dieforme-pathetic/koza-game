using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Loader : MonoBehaviour
{
    public Slider progressBar;

    void Start()
    {
        if (progressBar == null)
        {
            Debug.LogError("❌ Slider не назначен! Перетащите Slider в поле Progress Bar в инспекторе");
            return;
        }
        
        Debug.Log("✅ Slider найден, начинаю загрузку");
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("menu");

        while (!operation.isDone)
        {
            progressBar.value = operation.progress;
            yield return null;
        }
    }
}