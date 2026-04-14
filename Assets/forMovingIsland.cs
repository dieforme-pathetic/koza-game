using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    
    private Transform target;
    private Vector3 lastPos;

    void Start()
    {
        target = pointB;
        transform.position = pointA.position;
        lastPos = transform.position;
    }

    void Update()
    {
        // Двигаем платформу
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            target = target == pointA ? pointB : pointA;
        }
        
        // Смещение платформы за кадр
        Vector3 delta = transform.position - lastPos;
        lastPos = transform.position;
        
        // Толкаем всё, что на платформе
        if (delta.magnitude > 0)
        {
            Collider2D[] onPlatform = Physics2D.OverlapBoxAll(transform.position, 
                GetComponent<Collider2D>().bounds.size, 0);
            
            foreach (Collider2D col in onPlatform)
            {
                if (col.CompareTag("Player") && col.GetComponent<Rigidbody2D>() != null)
                {
                    col.GetComponent<Rigidbody2D>().MovePosition(col.GetComponent<Rigidbody2D>().position + (Vector2)delta);
                }
            }
        }
    }
}