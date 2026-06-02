using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterDeathDetector : MonoBehaviour
{
    public float respawnDelay = 1f;
    public string sandTag = "Ground";
    public string boatWallTag = "BoatWall";
    
    private CharacterMovement movement;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator animator;
    
    private bool isFalling = false;
    private float fallTimer = 0f;
    public float deathDelay = 0.1f;
    
    private List<SpriteRenderer> safeTiles; // Список безопасных объектов (Ground + BoatWall)
    
    void Start()
    {
        movement = GetComponent<CharacterMovement>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        safeTiles = new List<SpriteRenderer>();
        
        // Находим все объекты с тегом Ground
        GameObject[] groundObjects = GameObject.FindGameObjectsWithTag(sandTag);
        foreach (var obj in groundObjects)
        {
            SpriteRenderer s = obj.GetComponent<SpriteRenderer>();
            if (s != null) safeTiles.Add(s);
        }
        
        // Находим все объекты с тегом BoatWall
        GameObject[] wallObjects = GameObject.FindGameObjectsWithTag(boatWallTag);
        foreach (var obj in wallObjects)
        {
            SpriteRenderer s = obj.GetComponent<SpriteRenderer>();
            if (s != null) safeTiles.Add(s);
        }
        
        Debug.Log($"Найдено безопасных объектов (Ground + BoatWall): {safeTiles.Count}");
        
        enabled = false;
        Invoke(nameof(EnableDetector), 0.5f);
    }
    
    private void EnableDetector()
    {
        enabled = true;
    }
    
    void Update()
    {
        if (movement.isDead) return;
        
        // Проверяем в лодке ли коза
        if (movement.IsInBoat())
        {
            isFalling = false;
            fallTimer = 0f;
            return;
        }
        
        Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);
        bool onSafeTile = false;
        
        foreach (var tile in safeTiles)
        {
            if (tile == null) continue;
            
            Vector2 min = new Vector2(tile.bounds.min.x, tile.bounds.min.y);
            Vector2 max = new Vector2(tile.bounds.max.x, tile.bounds.max.y);
            
            if (pos2D.x >= min.x && pos2D.x <= max.x &&
                pos2D.y >= min.y && pos2D.y <= max.y)
            {
                onSafeTile = true;
                break;
            }
        }
        
        if (!onSafeTile)
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
        
        // Если в лодке — не умираем!
        if (movement.IsInBoat())
        {
            Debug.Log("Коза в лодке, смерть от воды отменена");
            yield break;
        }
        
        movement.isDead = true;
        
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        
        if (animator != null)
        {
            animator.SetTrigger("Drowning");
        }
        
        float deathAnimDuration = 0.5f;
        yield return new WaitForSeconds(deathAnimDuration);
        
        float t = 0f;
        while (t < respawnDelay)
        {
            sr.color = sr.color.a > 0 ? Color.clear : Color.red;
            yield return new WaitForSeconds(0.12f);
            t += 0.12f;
        }
        
        // ========== НАХОДИМ ТОЧКУ РЕСПАВНА ПО ТЕГУ ==========
        GameObject respawnObj = GameObject.FindGameObjectWithTag("Respawn");
        if (respawnObj != null)
        {
            transform.position = new Vector3(respawnObj.transform.position.x, respawnObj.transform.position.y, 0f);
            Debug.Log("Коза возродилась у точки респавна (Respawn)");
        }
        else
        {
            transform.position = new Vector3(0, 0, 0f);
            Debug.LogWarning("Точка респавна с тегом 'Respawn' не найдена! Коза возродилась в (0,0,0)");
        }
        
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = false;
        sr.color = Color.white;
        movement.isDead = false;
    }
}