using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private string levelName = "Game scene 3 04.05";
    
    public void LoadThisLevel()
    {
        SceneManager.LoadScene(levelName);
    }
}