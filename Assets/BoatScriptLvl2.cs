using UnityEngine;

public class Boat : MonoBehaviour
{
    public float speed = 5f;
    public Transform boatRespawnPoint;
    
    private CharacterMovement passenger;
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Collider2D boatCollider;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();
        
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        
        boatCollider = GetComponent<Collider2D>();
        if (boatCollider != null)
            boatCollider.isTrigger = true;
    }
    
    void FixedUpdate()
    {
        if (passenger != null && moveInput.magnitude > 0.01f)
        {
            // Новая позиция
            Vector2 newPos = rb.position + moveInput * speed * Time.fixedDeltaTime;
            
            // Проверяем, можно ли двигаться
            if (CanMoveTo(newPos))
            {
                rb.MovePosition(newPos);
            }
        }
    }
    
    bool CanMoveTo(Vector2 targetPos)
    {
        // Проверяем коллайдеры в новой позиции
        Collider2D[] hits = Physics2D.OverlapCircleAll(targetPos, 0.4f);
        
        foreach (Collider2D hit in hits)
        {
            if (hit == boatCollider) continue;
            if (hit.CompareTag("Player")) continue;
            
            // Лодка НЕ МОЖЕТ ехать по Ground
            if (hit.gameObject.layer == LayerMask.NameToLayer("ground"))
            {
                return false;
            }
            
            if (hit.CompareTag("Ground"))
            {
                return false;
            }
        }
        
        return true;
    }
    
    public void SetMovementInput(Vector2 input)
    {
        moveInput = input.magnitude < 0.05f ? Vector2.zero : input.normalized;
    }
    
    public void SetPassenger(CharacterMovement newPassenger)
    {
        passenger = newPassenger;
    }
    
    public bool HasPassenger()
    {
        return passenger != null;
    }
    
    public void TeleportToRespawn()
    {
        if (boatRespawnPoint != null)
            transform.position = boatRespawnPoint.position;
        else
        {
            GameObject respawnObj = GameObject.FindGameObjectWithTag("RespawnPoint");
            if (respawnObj != null)
                transform.position = respawnObj.transform.position + new Vector3(2f, 0, 0);
        }
        
        moveInput = Vector2.zero;
        
        if (passenger != null)
        {
            passenger.ExitBoat();
            passenger = null;
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && passenger == null)
        {
            CharacterMovement movement = other.GetComponent<CharacterMovement>();
            if (movement != null)
                movement.SetNearbyBoat(this, true);
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
}