using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    
    // ========== НАСТРОЙКИ СМЕРТИ ==========
    public string deathTag = "backGroundSea";    // Тег
    public LayerMask deathLayer;                  // Слой (выбрать в инспекторе!)
    public float respawnDelay = 1f;
    public string safeTag = "ground";
    
    private Vector3 startPosition;
    private bool isDead = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
    }

    void Update()
    {
        if (isDead) return;
        
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        if (isDead) return;
        
        Vector2 newPosition = rb.position + movement * (speed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckDeath(collision.gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckDeath(other.gameObject);
    }
    
    private void CheckDeath(GameObject obj)
    {
        if (isDead) return;
        
        // Проверка по тегу
        if (!string.IsNullOrEmpty(deathTag) && obj.CompareTag(deathTag) && !obj.CompareTag(safeTag))
        {
            Die();
            return;
        }
    }
    
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log("Коза умерла от касания " + deathTag);
        
        movement = Vector2.zero;
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
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
        
        if (rb != null)
        {
            rb.isKinematic = false;
        }
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
        
        isDead = false;
        Debug.Log("Коза воскресла!");
    }
}