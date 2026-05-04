using UnityEngine;
using System.Collections;

public class WaterDeathDetector : MonoBehaviour
{
    public float respawnDelay = 1f;
    public Transform respawnPoint;
    public string sandTag = "Ground"; // поставь этот тег на все досочки

    private CharacterMovement movement;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Vector3 startPos;
    private Animator animator;
    
    private bool isFalling = false;
    private float fallTimer = 0f;
    public float deathDelay = 0.1f; // Задержка перед смертью (можно настроить в инспекторе)

    // Кэшируем все песчаные объекты чтобы не искать каждый кадр
    private SpriteRenderer[] sandTiles;

    void Start()
    {
        movement = GetComponent<CharacterMovement>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPos = respawnPoint != null ? respawnPoint.position : transform.position;

        // Собираем все спрайты с тегом Ground
        GameObject[] sandObjects = GameObject.FindGameObjectsWithTag(sandTag);
        System.Collections.Generic.List<SpriteRenderer> srs = new();
        foreach (var obj in sandObjects)
        {
            SpriteRenderer s = obj.GetComponent<SpriteRenderer>();
            if (s != null) srs.Add(s);
        }
        sandTiles = srs.ToArray();
        Debug.Log("Найдено песчаных тайлов: " + sandTiles.Length);

        enabled = false;
        Invoke(nameof(EnableDetector), 0.5f);
        Debug.Log("WaterDeathDetector запустился. isDead = " + movement. isDead);
    }

    private void EnableDetector()
    {
        enabled = true;
        Debug.Log("Детектор включён. Позиция козы: " + transform.position);
        Debug.Log("Количество тайлов: " + sandTiles.Length);

        Vector3 pos = transform.position;
        foreach (var tile in sandTiles)
        {
            Debug.Log("Тайл: " + tile.name + " contains коза: " + tile.bounds.Contains(pos) + " | bounds: " + tile.bounds.min + " до " + tile.bounds.max);
        }
    }

    void Update()
    {
        if (movement.isDead) return;

        // Берём только X и Y позиции козы
        Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);
        bool onSand = false;

        foreach (var tile in sandTiles)
        {
            // Проверяем только по X и Y через 2D bounds
            Vector2 min = new Vector2(tile.bounds.min.x, tile.bounds.min.y);
            Vector2 max = new Vector2(tile.bounds.max.x, tile.bounds.max.y);

            if (pos2D.x >= min.x && pos2D.x <= max.x &&
                pos2D.y >= min.y && pos2D.y <= max.y)
            {
                onSand = true;
                break;
            }
        }

        if (!onSand)
        {
            if (!isFalling)
            {
                isFalling = true;
                fallTimer = 0f;
            }

            fallTimer += Time.deltaTime;

            if (fallTimer >= deathDelay)
            {
                StartCoroutine(DieAndRespawn());
            }
        }
        else
        {
            isFalling = false;
            fallTimer = 0f;
        }
            
    }

    private IEnumerator DieAndRespawn()
    {
        if (movement.isDead) yield break;
        movement.isDead = true;

        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        
        if (animator != null)
        {
            animator.SetTrigger("Drowning");
        }
        
        float deathAnimDuration = 0.5f; // Измените под длину вашей анимации смерти
        yield return new WaitForSeconds(deathAnimDuration);


        float t = 0f;
        while (t < respawnDelay)
        {
            sr.color = sr.color.a > 0 ? Color.clear : Color.red;
            yield return new WaitForSeconds(0.12f);
            t += 0.12f;
        }

        transform.position = new Vector3(startPos.x, startPos.y, 0f);
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = false;
        sr.color = Color.white;
        movement.isDead = false;
        
        // Возвращаем анимацию в состояние Idle (или другое состояние по умолчанию)
    }
}