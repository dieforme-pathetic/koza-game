using UnityEngine;
using UnityEngine.UI;

public class CharacterMovement : MonoBehaviour
{
    public float speed = 5f;
    public float grabRange = 1f;
    private Rigidbody2D rb;
    private Vector2 movement;
    public bool isDead = false;
    private Animator animator;
    private SpriteRenderer sr;
    public Transform respawnPoint;
    
    private PlatformMover attachedPlatform = null;
    private Vector2 platformOffset;
    private bool isHolding = false;
    
    // Для лодки
    private Boat nearbyBoat = null;
    private Boat currentBoat = null;
    private Vector2 boatOffset;
    
    public GameObject interactionPrompt;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        rb.gravityScale = 0f;
    
        // ОТКЛЮЧАЕМ ФИЗИКУ С МОНСТРАМИ
        Collider2D playerCollider = GetComponent<Collider2D>();
        SleepingMonster[] monsters = FindObjectsOfType<SleepingMonster>();
        foreach (var monster in monsters)
        {
            Collider2D monsterCollider = monster.GetComponent<Collider2D>();
            if (playerCollider != null && monsterCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, monsterCollider, true);
            }
        }
    
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
    
    void Update()
    {
        if (isDead) return;
    
        // ========== ЛОГИКА ЛОДКИ (когда внутри) ==========
        if (currentBoat != null)
        {
            Vector2 boatMovement = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ).normalized;
        
            currentBoat.SetMovementInput(boatMovement);
        
            // ВЫХОД из лодки по ПРОБЕЛУ (теперь тоже пробел)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ExitBoat();
            }
            return;
        }
    
        // Обновляем UI подсказку
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(nearbyBoat != null);
        }
    
        // ========== ЛОГИКА ПЛАТФОРМЫ И ЛОДКИ (ПРОБЕЛ) ==========
    
        // НАЖАТИЕ ПРОБЕЛА (один раз)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Сначала проверяем - есть ли лодка рядом
            if (nearbyBoat != null)
            {
                BoardBoat(nearbyBoat);
                return; // Выходим, чтобы не приклеиваться к платформе
            }
        
            // Если лодки нет - приклеиваемся к платформе
            if (!isHolding)
            {
                isHolding = true;
                TryAttachToPlatform();
            }
        }
    
        // ОТПУСКАНИЕ ПРОБЕЛА (для отклеивания от платформы)
        if (Input.GetKeyUp(KeyCode.Space))
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
        if (currentBoat != null) return;
        
        if (attachedPlatform == null)
        {
            rb.MovePosition(rb.position + movement * (speed * Time.fixedDeltaTime));
        }
    }
    
    void LateUpdate()
    {
        if (currentBoat != null)
        {
            Vector2 targetPos = (Vector2)currentBoat.transform.position + boatOffset;
            rb.MovePosition(targetPos);
            return;
        }
        
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
    }
    
    private void AttachToPlatform(PlatformMover platform)
    {
        attachedPlatform = platform;
        platformOffset = (Vector2)transform.position - platform.GetPosition();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
    }
    
    private void DetachFromPlatform()
    {
        if (attachedPlatform == null) return;
        attachedPlatform = null;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;
    }
    
    // ========== МЕТОДЫ ДЛЯ ЛОДКИ ==========
    public void SetNearbyBoat(Boat boat, bool isNear)
    {
        if (currentBoat != null) return;
        nearbyBoat = isNear ? boat : null;
    }
    
    public void BoardBoat(Boat boat)
    {
        if (currentBoat != null || boat == null) return;
        
        currentBoat = boat;
        nearbyBoat = null;
        boatOffset = new Vector2(0f, 0.5f);
        
        if (animator != null)
            animator.enabled = false;
        
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        
        transform.SetParent(boat.transform);
        transform.localPosition = new Vector3(0, 0.5f, 0);
        
        boat.SetPassenger(this);
        
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        
        Debug.Log("Коза села в лодку!");
    }
    
    public void ExitBoat()
    {
        if (currentBoat == null) return;
        
        transform.SetParent(null);
        
        if (animator != null)
            animator.enabled = true;
        
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;
        
        currentBoat.SetPassenger(null);
        currentBoat = null;
        
        Debug.Log("Коза вышла из лодки!");
    }
    
    public bool IsInBoat()
    {
        return currentBoat != null;
    }
    
    public bool IsAttachedToPlatform(PlatformMover platform)
    {
        return attachedPlatform == platform && isHolding;
    }
    
    // ========== СМЕРТЬ ОТ МОНСТРА ==========
    public void DieByMonster()
    {
        if (isDead) return;
        
        isDead = true;
        
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;
        
        if (animator != null)
            animator.SetTrigger("Drowning");
        
        if (attachedPlatform != null)
        {
            attachedPlatform = null;
            isHolding = false;
        }
        
        if (currentBoat != null)
        {
            currentBoat.SetPassenger(null);
            currentBoat = null;
        }
        
        StartCoroutine(MonsterDeathRespawn());
    }
    
    private System.Collections.IEnumerator MonsterDeathRespawn()
    {
        yield return new WaitForSeconds(0.5f);
        
        float respawnDelay = 1f;
        float t = 0f;
        while (t < respawnDelay)
        {
            if (sr != null)
                sr.color = sr.color.a > 0 ? Color.clear : Color.red;
            yield return new WaitForSeconds(0.12f);
            t += 0.12f;
        }
        
        // Находим точку респавна
        Vector3 respawnPos;
        if (respawnPoint != null)
        {
            respawnPos = respawnPoint.position;
        }
        else
        {
            GameObject respawnObj = GameObject.FindGameObjectWithTag("RespawnPoint");
            if (respawnObj != null)
                respawnPos = respawnObj.transform.position;
            else
                respawnPos = Vector3.zero;
        }
        
        // Перемещаем козу
        transform.position = respawnPos;
        
        // Перемещаем все лодки
        Boat[] allBoats = FindObjectsOfType<Boat>();
        foreach (Boat boat in allBoats)
        {
            if (boat != null)
            {
                if (boat.boatRespawnPoint != null)
                    boat.transform.position = boat.boatRespawnPoint.position;
                else
                    boat.transform.position = respawnPos + new Vector3(2f, 0, 0);
                
                boat.SetMovementInput(Vector2.zero);
                
                if (boat.HasPassenger())
                    boat.SetPassenger(null);
            }
        }
        
        if (sr != null)
            sr.color = Color.white;
        
        rb.isKinematic = false;
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = true;
        
        isDead = false;
        
        if (animator != null)
            animator.Play("Idle");
        
        SleepingMonster[] monsters = FindObjectsOfType<SleepingMonster>();
        foreach (var monster in monsters)
        {
            monster.ResetMonster();
        }
        
        Debug.Log("Коза возродилась!");
    }
    
    // ========== АНИМАЦИЯ ==========
    private void UpdateAnimations()
    {
        if (animator == null) return;
    
        float currentSpeed = movement.magnitude;
        animator.SetFloat("speed", currentSpeed);
    
        if (sr == null) sr = GetComponent<SpriteRenderer>();
    
        if (movement.x > 0.01f)
        {
            sr.flipX = false;
        }
        else if (movement.x < -0.01f)
        {
            sr.flipX = true;
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, grabRange);
    }
}