using UnityEngine;

public class Boat : MonoBehaviour
{
    [Header("Движение")]
    public float speed = 5f;
    
    [Header("Спрайт козы (стоячий) — опционально")]
    public Sprite defaultGoatSprite;
    
    private CharacterMovement passenger; // ссылка на козу-пассажира
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Collider2D boatTrigger;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        rb.gravityScale = 0f;
        
        // Получаем коллайдер-триггер для обнаружения козы
        boatTrigger = GetComponent<Collider2D>();
        if (boatTrigger != null)
            boatTrigger.isTrigger = true;
    }
    
    void Update()
    {
        // Движение только если есть пассажир
        if (passenger != null)
        {
            // Двигаем лодку по вводу
            if (moveInput.magnitude > 0.01f)
            {
                Vector2 newPos = rb.position + moveInput * speed * Time.deltaTime;
                rb.MovePosition(newPos);
            }
        }
    }
    
    void FixedUpdate()
    {
        // Альтернативное движение в FixedUpdate
        if (passenger != null && moveInput.magnitude > 0.01f)
        {
            Vector2 newPos = rb.position + moveInput * speed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
        }
    }
    
    // Получить ввод от козы
    public void SetMovementInput(Vector2 input)
    {
        moveInput = input.normalized;
    }
    
    // Установить пассажира (вызывается из CharacterMovement)
    public void SetPassenger(CharacterMovement newPassenger)
    {
        passenger = newPassenger;
        
        if (passenger != null)
        {
            // Меняем спрайт на стоячий (опционально)
            SpriteRenderer sr = passenger.GetComponent<SpriteRenderer>();
            if (sr != null && defaultGoatSprite != null)
            {
                sr.sprite = defaultGoatSprite;
            }
            Debug.Log("Лодка: пассажир сел");
        }
        else
        {
            Debug.Log("Лодка: пассажир вышел");
        }
    }
    
    // Обработка входа в зону лодки
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterMovement movement = other.GetComponent<CharacterMovement>();
            if (movement != null && passenger == null)
            {
                movement.SetNearbyBoat(this, true);
                Debug.Log("Коза рядом с лодкой! Нажми E, чтобы сесть");
            }
        }
    }
    
    // Обработка выхода из зоны лодки
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterMovement movement = other.GetComponent<CharacterMovement>();
            if (movement != null && passenger == null)
            {
                movement.SetNearbyBoat(this, false);
                Debug.Log("Коза отошла от лодки");
            }
        }
    }
    
    public bool HasPassenger()
    {
        return passenger != null;
    }
}