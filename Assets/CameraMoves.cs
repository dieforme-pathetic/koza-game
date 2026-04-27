using UnityEngine;

public class CameraMoves : MonoBehaviour
{
    public Transform target;           // Коза, за которой следим
    public Transform background;       // Фон (перетащи Background сюда)
    public float cameraSize = 5f;      // Размер камеры (чем меньше, тем ближе)
    
    private Camera cam;
    private float minX, maxX, minY, maxY;
    
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = cameraSize;
        
        // Автоматически вычисляем границы по фону
        if (background != null)
        {
            SpriteRenderer bgRenderer = background.GetComponent<SpriteRenderer>();
            if (bgRenderer != null)
            {
                float halfHeight = cameraSize;
                float halfWidth = halfHeight * (Screen.width / (float)Screen.height);
                
                Bounds bgBounds = bgRenderer.bounds;
                minX = bgBounds.min.x + halfWidth;
                maxX = bgBounds.max.x - halfWidth;
                minY = bgBounds.min.y + halfHeight;
                maxY = bgBounds.max.y - halfHeight;
            }
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        float targetX = Mathf.Clamp(target.position.x, minX, maxX);
        float targetY = Mathf.Clamp(target.position.y, minY, maxY);
        
        transform.position = new Vector3(targetX, targetY, -10);
    }
}