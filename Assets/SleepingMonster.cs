using UnityEngine;

public class SleepingMonster : MonoBehaviour
{
    [Header("Настройки пробуждения")]
    public float wakeUpRange = 4f;
    public float attackRange = 0.8f;
    
    [Header("Настройки движения")]
    public float chaseSpeed = 2.5f;
    
    private Transform player;
    private bool isAwake = false;
    private bool isKilling = false;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector3 startPosition;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            
            Collider2D monsterCollider = GetComponent<Collider2D>();
            Collider2D playerCollider = playerObj.GetComponent<Collider2D>();
            if (monsterCollider != null && playerCollider != null)
            {
                Physics2D.IgnoreCollision(monsterCollider, playerCollider, true);
            }
        }
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        
        // Начальное состояние - спим
        if (animator != null)
        {
            animator.SetBool("IsAwake", false);
            animator.SetBool("IsKilling", false);
        }
    }
    
    void Update()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (isKilling) return;
        
        if (!isAwake && distanceToPlayer <= wakeUpRange)
        {
            WakeUp();
        }
        
        if (isAwake && !isKilling)
        {
            ChasePlayer();
            
            if (distanceToPlayer <= attackRange)
            {
                KillPlayer();
            }
        }
    }
    
    void WakeUp()
    {
        isAwake = true;
        
        if (animator != null)
        {
            animator.SetBool("IsAwake", true);
            animator.SetBool("IsKilling", false);
        }
        
        Debug.Log("Монстр проснулся!");
    }
    
    void ChasePlayer()
    {
        if (player == null) return;
        
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 newPos = rb.position + direction * chaseSpeed * Time.deltaTime;
        rb.MovePosition(newPos);
        
        if (direction.x != 0 && spriteRenderer != null)
        {
            spriteRenderer.flipX = direction.x < 0;
        }
    }
    
    void KillPlayer()
    {
        if (isKilling) return;
        
        isKilling = true;
        
        rb.linearVelocity = Vector2.zero;
        
        if (animator != null)
        {
            animator.SetBool("IsKilling", true);
            animator.SetBool("IsAwake", false);
        }
        
        // Убиваем игрока
        if (player != null)
        {
            CharacterMovement playerMovement = player.GetComponent<CharacterMovement>();
            if (playerMovement != null && !playerMovement.isDead)
            {
                playerMovement.DieByMonster();
                Debug.Log("Монстр убил игрока!");
            }
        }
    }
    
    public void ResetMonster()
    {
        // Полностью сбрасываем состояние
        isAwake = false;
        isKilling = false;
        
        transform.position = startPosition;
        
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
        
        if (animator != null)
        {
            animator.SetBool("IsAwake", false);
            animator.SetBool("IsKilling", false);
            animator.Rebind(); // ПРИНУДИТЕЛЬНЫЙ СБРОС АНИМАТОРА
            animator.Update(0f);
        }
        
        // Обновляем ссылку на игрока
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        
        Debug.Log("Монстр вернулся в спящее состояние и сбросил анимацию убийства");
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, wakeUpRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}