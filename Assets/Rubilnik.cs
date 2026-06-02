using UnityEngine;

public class PickupItem : MonoBehaviour
{
    private bool isPickedUp = false;
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Collider2D itemCollider;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        itemCollider = GetComponent<Collider2D>();
        
        if (itemCollider != null)
            itemCollider.isTrigger = true;
        
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        Debug.Log("✅ Рубильник появился в мире! (это сообщение должно быть ТОЛЬКО после ввода кода)");
    }
    
    void Update()
    {
        if (isPickedUp && player != null)
        {
            transform.position = player.position + new Vector3(0, 1.2f, 0);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isPickedUp) return;
        
        if (other.CompareTag("Player"))
        {
            PickUp();
        }
    }
    
    void PickUp()
    {
        isPickedUp = true;
        
        if (itemCollider != null)
            itemCollider.enabled = false;
        
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(1, 1, 1, 0.7f);
        
        Debug.Log("✅ Рубильник взят! Иди ко второму сундуку.");
    }
    
    public bool IsPickedUp()
    {
        return isPickedUp;
    }
}