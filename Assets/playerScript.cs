using UnityEngine;

public class CharacterMovement1 : MonoBehaviour 
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    
    public float respawnDelay = 1f;
    
    // Выбирай слои в инспекторе!
    public LayerMask waterLayer;   // Слой воды (слой 4)
    public LayerMask groundLayer;  // Слой земли (слой 6)
    
    private Vector3 startPosition;
    private bool isDead = false;
    private SpriteRenderer spriteRenderer;
    
    // ========== ДЛЯ АНИМАЦИЙ ==========
    private Animator animator;
    
    // ========== ДЛЯ ПЛАТФОРМ ==========
    private Transform currentPlatform;  // На какой платформе сейчас стоит
    private Vector3 lastPlatformPosition; // Предыдущая позиция платформы
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
        
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("На объекте нет компонента Animator! Анимации не будут работать.");
        }
        
        if (waterLayer == 0) waterLayer = LayerMask.GetMask("Water");
        if (groundLayer == 0) groundLayer = LayerMask.GetMask("ground");
    }

    void Update()
    {
        if (isDead) return;
        
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        
        UpdateAnimations();
    }
    
    private void UpdateAnimations()
    {
        if (animator == null) return;
        
        float currentSpeed = movement.magnitude;
        animator.SetFloat("speed", currentSpeed);
        
        // Поворот спрайта
        if (movement.x > 0.01f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (movement.x < -0.01f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;
        
        // Движение козы
        Vector2 newPosition = rb.position + movement * (speed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);
        
        // Движение вместе с платформой
        if (currentPlatform != null)
        {
            Vector3 platformDelta = currentPlatform.position - lastPlatformPosition;
            rb.MovePosition(rb.position + (Vector2)platformDelta);
        }
        
        // Обновляем последнюю позицию платформы
        if (currentPlatform != null)
        {
            lastPlatformPosition = currentPlatform.position;
        }
    }
    
    // Когда коза наступает на платформу (землю)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        
        // Проверяем слой Ground (6)
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            currentPlatform = collision.transform;
            lastPlatformPosition = currentPlatform.position;
            Debug.Log($"Встала на платформу: {collision.gameObject.name}");
        }
        
        // Проверяем смерть от воды
        if (((1 << collision.gameObject.layer) & waterLayer) != 0)
        {
            Debug.Log("Это ВОДА по слою! Умираю!");
            Die();
        }
    }
    
    // Когда коза сходит с платформы
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            if (currentPlatform == collision.transform)
            {
                currentPlatform = null;
                Debug.Log($"Сошла с платформы: {collision.gameObject.name}");
            }
        }
    }
    
    // Для триггеров (вода может быть триггером)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        
        if (((1 << other.gameObject.layer) & waterLayer) != 0)
        {
            Debug.Log("Это ВОДА по слою! Умираю!");
            Die();
        }
    }
    
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log("=== КОЗА УМЕРЛА ===");
        
        movement = Vector2.zero;
        
        if (animator != null)
        {
            animator.SetFloat("speed", 0f);
        }
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }
        
        Invoke(nameof(Respawn), respawnDelay);
    }
    
    private void Respawn()
    {
        transform.position = startPosition;
        currentPlatform = null;
        
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
        
        isDead = false;
        Debug.Log("=== КОЗА ВОСКРЕСЛА ===");
    }
}