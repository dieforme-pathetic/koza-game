using UnityEngine;

public class Boat : MonoBehaviour
{
    public float speed = 5f;
    public Transform boatRespawnPoint;
    
    [Header("Настройки дебаггера")]
    public bool showDebugMessages = true;
    public bool showDebugGizmos = true;
    
    [Header("Радиус посадки")]
    public float pickupRadius = 2f;
    
    [Header("Обнаружение стен")]
    public float wallCheckDistance = 0.8f;
    
    private CharacterMovement passenger;
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Collider2D boatCollider;
    private Vector3 startPosition;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();
        
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        
        CircleCollider2D trigger = GetComponent<CircleCollider2D>();
        if (trigger == null)
            trigger = gameObject.AddComponent<CircleCollider2D>();
        trigger.isTrigger = true;
        trigger.radius = pickupRadius;
        boatCollider = trigger;
        
        startPosition = transform.position;
        
        if (showDebugMessages)
            Debug.Log("[ЛОДКА] Инициализирована");
    }
    
    void FixedUpdate()
    {
        if (passenger != null && moveInput.magnitude > 0.01f)
        {
            Vector2 newPos = rb.position + moveInput * speed * Time.fixedDeltaTime;
            
            if (CanMoveTo(newPos))
            {
                rb.MovePosition(newPos);
            }
        }
    }
    
    bool CanMoveTo(Vector2 targetPos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(targetPos, 0.5f);
        
        foreach (Collider2D hit in hits)
        {
            if (hit == boatCollider) continue;
            if (hit.CompareTag("Player")) continue;
            
            if (hit.CompareTag("Wall") || hit.CompareTag("BoatWall"))
            {
                if (showDebugMessages)
                    Debug.Log($"[ЛОДКА] ❌ Врезалась в: {hit.name} (тег: {hit.tag})");
                return false;
            }
        }
        
        return true;
    }
    
    public void SetMovementInput(Vector2 input)
    {
        moveInput = input.magnitude < 0.05f ? Vector2.zero : input.normalized;
    }
    
    public Vector2 GetMovementDirection()
    {
        return moveInput;
    }
    
    public void SetPassenger(CharacterMovement newPassenger)
    {
        passenger = newPassenger;
        if (showDebugMessages)
            Debug.Log($"[ЛОДКА] Пассажир {(passenger != null ? "сел" : "вышел")}");
    }
    
    public bool HasPassenger()
    {
        return passenger != null;
    }
    
    // ОБЩИЙ МЕТОД ДЛЯ РЕСПАВНА ЛОДКИ (вызывается при любой смерти козы)
    public void RespawnBoat()
    {
        // Находим точку респавна
        Vector3 respawnPos;
        
        // Своя точка респавна лодки
        if (boatRespawnPoint != null)
        {
            respawnPos = boatRespawnPoint.position;
            if (showDebugMessages)
                Debug.Log($"[ЛОДКА] Респавн на своей точке: {respawnPos}");
        }
        else
        {
            // Ищем точку респавна козы
            GameObject respawnObj = GameObject.FindGameObjectWithTag("RespawnPoint");
            if (respawnObj != null)
            {
                respawnPos = respawnObj.transform.position + new Vector3(2f, 0, 0);
                if (showDebugMessages)
                    Debug.Log($"[ЛОДКА] Респавн у точки козы: {respawnPos}");
            }
            else
            {
                respawnPos = startPosition;
                if (showDebugMessages)
                    Debug.Log($"[ЛОДКА] Респавн на стартовую позицию: {respawnPos}");
            }
        }
        
        // Телепортируем лодку
        transform.position = respawnPos;
        
        // Сбрасываем движение
        moveInput = Vector2.zero;
        
        // Высаживаем пассажира если есть
        if (passenger != null)
        {
            passenger.ExitBoat();
            passenger = null;
        }
        
        // Сбрасываем скорость
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
        
        if (showDebugMessages)
            Debug.Log("[ЛОДКА] Респавн завершён!");
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && passenger == null)
        {
            CharacterMovement movement = other.GetComponent<CharacterMovement>();
            if (movement != null)
            {
                movement.SetNearbyBoat(this, true);
                if (showDebugMessages)
                    Debug.Log("[ЛОДКА] Коза рядом! Нажми ПРОБЕЛ, чтобы сесть");
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && passenger == null)
        {
            CharacterMovement movement = other.GetComponent<CharacterMovement>();
            if (movement != null)
                movement.SetNearbyBoat(this, false);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
        
        if (boatRespawnPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(boatRespawnPoint.position, 0.5f);
        }
    }
}