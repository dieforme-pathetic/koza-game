using UnityEngine;
using UnityEngine.UI; // для UI подсказки (опционально)

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
    
    // Для лодки
    private Boat nearbyBoat = null;      // лодка рядом
    private Boat currentBoat = null;     // лодка, в которой сидим
    private Vector2 boatOffset;
    
    // UI подсказка (опционально)
    public GameObject interactionPrompt;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        animator = GetComponent<Animator>();
        rb.gravityScale = 0f;
        
        if (animator == null)
        {
            Debug.LogWarning("На объекте нет компонента Animator! Анимации не будут работать.");
        }
        
        // Скрываем подсказку при старте
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;
        
        // ========== ЛОГИКА ЛОДКИ ==========
        
        // Если в лодке
        if (currentBoat != null)
        {
            // Управление лодкой через WASD
            Vector2 boatMovement = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ).normalized;
            
            // Передаём управление лодке
            if (currentBoat != null)
            {
                currentBoat.SetMovementInput(boatMovement);
            }
            
            // Выход из лодки по E
            if (Input.GetKeyDown(KeyCode.E))
            {
                ExitBoat();
            }
            return; // Остальная логика движения не нужна
        }
        
        // Если НЕ в лодке, проверяем наличие лодки рядом для посадки
        if (nearbyBoat != null && Input.GetKeyDown(KeyCode.E))
        {
            BoardBoat(nearbyBoat);
            return;
        }
        
        // Обновляем UI подсказку
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(nearbyBoat != null);
        }
        
        // ========== ЛОГИКА ПЛАТФОРМЫ ==========
        
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
        
        // Движение
        movement = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;
        
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        if (isDead) return;
        
        // Если в лодке — не двигаемся сами
        if (currentBoat != null) return;
        
        // Если НЕ приклеены — двигаемся сами
        if (attachedPlatform == null)
        {
            rb.MovePosition(rb.position + movement * (speed * Time.fixedDeltaTime));
        }
    }
    
    void LateUpdate()
    {
        // Если в лодке — двигаемся вместе с лодкой
        if (currentBoat != null)
        {
            Vector2 targetPos = (Vector2)currentBoat.transform.position + boatOffset;
            rb.MovePosition(targetPos);
            return;
        }
        
        // Если приклеены к платформе
        if (attachedPlatform != null && isHolding)
        {
            Vector2 targetPos = attachedPlatform.GetPosition() + platformOffset;
            rb.MovePosition(targetPos);
        }
    }
    
    // ========== МЕТОДЫ ДЛЯ ПЛАТФОРМЫ ==========
    
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
        
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        
        Debug.Log("Приклеились к платформе!");
    }
    
    private void DetachFromPlatform()
    {
        if (attachedPlatform == null) return;
        
        attachedPlatform = null;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;
        
        Debug.Log("Отклеились от платформы");
    }
    
    // ========== МЕТОДЫ ДЛЯ ЛОДКИ ==========
    
    // Вызывается когда коза заходит в триггер лодки
    public void SetNearbyBoat(Boat boat, bool isNear)
    {
        if (currentBoat != null) return; // уже в лодке, игнорируем
        nearbyBoat = isNear ? boat : null;
    }
    
    // Посадка в лодку
    // Посадка в лодку
    public void BoardBoat(Boat boat)
    {
        if (currentBoat != null) return;
        if (boat == null) return;
    
        currentBoat = boat;
        nearbyBoat = null;
    
        // ВАЖНО: смещение = 0, чтобы коза села в ЦЕНТР лодки
        boatOffset = Vector2.zero + new Vector2(0f, 1.5f);
    
        // Отключаем анимацию
        if (animator != null)
            animator.enabled = false;
    
        // Отключаем управление
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
    
        // Отключаем коллайдер козы (чтобы не мешал лодке)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;
    
        // Прикрепляем к лодке визуально и ставим в ЦЕНТР
        transform.SetParent(boat.transform);
        transform.localPosition = Vector3.zero; // ← ЦЕНТР ЛОДКИ
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    
        // Сообщаем лодке, что пассажир сел
        boat.SetPassenger(this);
    
        // Скрываем подсказку
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    
        Debug.Log("🐐 Коза села в лодку! (в центре) Нажми E, чтобы выйти");
    }
    
    // Выход из лодки
    public void ExitBoat()
    {
        if (currentBoat == null) return;
        
        // Открепляем от лодки
        transform.SetParent(null);
        
        // Включаем анимацию
        if (animator != null)
            animator.enabled = true;
        
        // Включаем коллайдер
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = true;
        
        // Возвращаем динамическое тело
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;
        
        // Сообщаем лодке, что пассажир вышел
        currentBoat.SetPassenger(null);
        
        currentBoat = null;
        
        Debug.Log("🐐 Коза вышла из лодки!");
    }
    
    public bool IsInBoat()
    {
        return currentBoat != null;
    }
    
    public bool IsAttachedToPlatform(PlatformMover platform)
    {
        return attachedPlatform == platform;
    }
    
    // ========== АНИМАЦИЯ ==========
    
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
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, grabRange);
    }
}