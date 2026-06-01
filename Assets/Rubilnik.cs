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
        
        // Настраиваем коллайдер как триггер
        if (itemCollider != null)
            itemCollider.isTrigger = true;
        
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }
    
    void Update()
    {
        if (isPickedUp && player != null)
        {
            // Летаем за игроком
            transform.position = player.position + new Vector3(0, 1.2f, 0);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isPickedUp) return;
        
        if (other.CompareTag("Player"))
        {
            // АВТОМАТИЧЕСКИ ПОДБИРАЕМ
            PickUp();
        }
    }
    
    void PickUp()
    {
        isPickedUp = true;
        
        // Отключаем коллайдер
        if (itemCollider != null)
            itemCollider.enabled = false;
        
        // Делаем полупрозрачным
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(1, 1, 1, 0.7f);
        
        Debug.Log("Рубильник автоматически подобран! Иди ко второму сундуку");
    }
    
    public bool IsPickedUp()
    {
        return isPickedUp;
    }
}