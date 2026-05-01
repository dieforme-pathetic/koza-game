using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    public bool isDead = false;
    private Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Принудительно Z = 0 — критично для 2D физики
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("На объекте нет компонента Animator! Анимации не будут работать.");
        }
    }

    void Update()
    {
        if (isDead) return;
        movement = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        if (isDead) return;
        rb.MovePosition(rb.position + movement * (speed * Time.fixedDeltaTime));
    }
    
    private void UpdateAnimations()
    {
        if (animator == null) return;
        
        float currentSpeed = movement.magnitude;
        animator.SetFloat("speed", currentSpeed);
        
        // Поворот спрайта
        if (movement.x > 0.01f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (movement.x < -0.01f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}