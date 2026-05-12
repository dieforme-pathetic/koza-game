using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float speed = 5f;
    public float grabRange = 1f;
    private Rigidbody2D rb;
    private Vector2 movement;
    public bool isDead = false;
    private Animator animator;
    
    private PlatformMover attachedPlatform = null;
    private Vector2 platformOffset;
    private bool isHolding = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        animator = GetComponent<Animator>();
        rb.gravityScale = 0f; // топ-даун, гравитация не нужна
        
        if (animator == null)
        {
            Debug.LogWarning("На объекте нет компонента Animator! Анимации не будут работать.");
        }
    }

    void Update()
    {
        if (isDead) return;
        
        // Пробел зажат — приклеиваемся к платформе
        if (Input.GetKey(KeyCode.Space))
        {
            if (!isHolding)
            {
                isHolding = true;
                TryAttachToPlatform();
            }
        }
        else
        {
            if (isHolding)
            {
                isHolding = false;
                DetachFromPlatform();
            }
        }
        
        // Движение: всегда, если не мертва (даже на платформе!)
        movement = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;
        
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        if (isDead) return;
        
        // Если НЕ приклеены — двигаемся сами
        if (attachedPlatform == null)
        {
            rb.MovePosition(rb.position + movement * (speed * Time.fixedDeltaTime));
        }
        // Если приклеены — не двигаемся, платформа тащит (через LateUpdate)
    }
    
    void LateUpdate()
    {
        // Только если ПРИКЛЕЕНЫ к платформе (зажат пробел)
        if (attachedPlatform != null && isHolding)
        {
            Vector2 targetPos = attachedPlatform.GetPosition() + platformOffset;
            rb.MovePosition(targetPos);
        }
    }
    
    private void TryAttachToPlatform()
    {
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, grabRange);
        
        foreach (Collider2D col in nearby)
        {
            PlatformMover platform = col.GetComponent<PlatformMover>();
            if (platform != null)
            {
                AttachToPlatform(platform);
                return;
            }
        }
        
        Debug.Log("Нет платформы рядом!");
    }
    
    private void AttachToPlatform(PlatformMover platform)
    {
        attachedPlatform = platform;
        platformOffset = (Vector2)transform.position - platform.GetPosition();
        
        // При приклеивании делаем кинематическим (чтобы платформа тащила)
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        
        Debug.Log("Приклеились к платформе!");
    }

    private void DetachFromPlatform()
    {
        if (attachedPlatform == null) return;
        
        // Отклеиваемся, но остаёмся на платформе
        attachedPlatform = null;
        
        // Возвращаем динамическое тело (чтобы можно было бегать)
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;
        
        Debug.Log("Отклеились от платформы, но остались на ней");
    }
    
    private void UpdateAnimations()
    {
        if (animator == null) return;
        
        float currentSpeed = movement.magnitude;
        animator.SetFloat("speed", currentSpeed);
        
        if (movement.x > 0.01f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (movement.x < -0.01f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    
    public bool IsAttachedToPlatform(PlatformMover platform)
    {
        return attachedPlatform == platform;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, grabRange);
    }
}