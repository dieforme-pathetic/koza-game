using UnityEngine;
using UnityEngine.SceneManagement; // ОБЯЗАТЕЛЬНО для работы со сценами

public class SceneChanger : MonoBehaviour
{
    // Эту функцию мы привяжем к кнопке. 
    // В поле sceneName мы напишем точное название сцены, куда хотим перейти.
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
