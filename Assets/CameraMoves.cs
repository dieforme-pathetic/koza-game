using UnityEngine;

public class CameraMoves : MonoBehaviour
{
    public Transform target;
    public Transform background;
    public float cameraSize = 5f;
    
    private Camera cam;
    private float minX, maxX, minY, maxY;
    
    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
            cam = Camera.main;
        
        cam.orthographicSize = cameraSize;
        
        // Автоматически ищем цель если не назначена
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                target = playerObj.transform;
        }
        
        UpdateBounds();
    }
    
    void UpdateBounds()
    {
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
        // Если цель пропала, ищем козу по тегу КАЖДЫЙ КАДР
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                target = playerObj.transform;
            else
                return;
        }
        
        UpdateBounds();
        
        float targetX = Mathf.Clamp(target.position.x, minX, maxX);
        float targetY = Mathf.Clamp(target.position.y, minY, maxY);
        
        transform.position = new Vector3(targetX, targetY, -10);
    }
}