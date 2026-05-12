using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    
    private Transform target;
    private Vector3 lastPos;
    private Collider2D platformCollider;

    void Start()
    {
        target = pointB;
        transform.position = pointA.position;
        platformCollider = GetComponent<Collider2D>();
        
        // Z = 0 для 2D
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        
        // ВАЖНО: инициализируем lastPos ТОЛЬКО после установки позиции
        lastPos = transform.position;
    }

    void Update()
    {
        // Сохраняем предыдущую позицию ДО движения
        Vector3 oldPos = transform.position;
        
        // Двигаем платформу
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        
        // Вычисляем реальное смещение за этот кадр
        Vector3 delta = transform.position - oldPos;
        
        // Проверяем смену цели
        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            target = target == pointA ? pointB : pointA;
        }
        
        // Толкаем всё, что на платформе (ТОЛЬКО если реально сдвинулись)
        if (delta.magnitude > 0.001f && platformCollider != null)
        {
            Collider2D[] onPlatform = Physics2D.OverlapBoxAll(transform.position, platformCollider.bounds.size, 0);
            
            foreach (Collider2D col in onPlatform)
            {
                if (col.CompareTag("Player"))
                {
                    CharacterMovement movement = col.GetComponent<CharacterMovement>();
                    Rigidbody2D otherRb = col.GetComponent<Rigidbody2D>();
                    
                    // Пропускаем приклеенную козу (её LateUpdate сам подвинет)
                    if (movement != null && movement.IsAttachedToPlatform(this))
                    {
                        continue;
                    }
                    
                    // Толкаем всех остальных
                    if (otherRb != null)
                    {
                        otherRb.MovePosition(otherRb.position + (Vector2)delta);
                    }
                }
            }
        }
    }
    
    public Vector2 GetPosition()
    {
        return transform.position;
    }
}