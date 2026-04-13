using UnityEngine;

public class RotateOnClick : MonoBehaviour
{
    void Update()
    {
        // Это сообщение вы уже видите
        // Debug.Log("Update работает");

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("=== КЛИК МЫШЬЮ ОБНАРУЖЕН! ===");
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            
            Debug.Log("Позиция мыши в мире: " + mousePos2D);
            
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            
            if (hit.collider != null)
            {
                Debug.Log("Луч попал в объект: " + hit.collider.gameObject.name);
                
                if (hit.collider.gameObject == gameObject)
                {
                    Debug.Log("Луч попал ИМЕННО в Square! Поворачиваю...");
                    transform.Rotate(0f, 0f, 90f);
                    Debug.Log("Поворот выполнен! Новый угол Z: " + transform.rotation.eulerAngles.z);
                }
                else
                {
                    Debug.Log("Луч попал в ДРУГОЙ объект: " + hit.collider.gameObject.name);
                }
            }
            else
            {
                Debug.Log("Луч НИКУДА не попал! (клик мимо квадрата)");
            }
        }
    }
}