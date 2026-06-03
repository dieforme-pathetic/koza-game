using UnityEngine;

public class LevelController : MonoBehaviour
{
    // Ссылка на наше скрытое окно победы
    public GameObject victoryPanel;

    // Функция срабатывает, когда коза входит в триггер финиша
    // (Если игра 2D, замените на OnTriggerEnter2D(Collider2D other))
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что в триггер зашла именно коза (игрок)
        if (other.CompareTag("Player"))
        {
            WinLevel();
        }
    }

    void WinLevel()
    {
        if (victoryPanel != null)
        {
            // Включаем окно победы на экране
            victoryPanel.SetActive(true);

            // Опционально: останавливаем время в игре, чтобы коза больше не двигалась
            Time.timeScale = 0f;
        }
    }

    // ВАЖНО: Если мы останавливали время, его нужно вернуть в норму при смене сцены
    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
