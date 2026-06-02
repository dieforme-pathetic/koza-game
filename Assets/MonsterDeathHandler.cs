using UnityEngine;
using System.Collections;

public class MonsterDeathHandler : MonoBehaviour
{
    [Header("Настройки смерти")]
    public float deathAnimDuration = 0.5f;
    public float respawnDelay = 1f;
    public Transform respawnPoint;
    
    private CharacterMovement movement;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D col;
    
    void Start()
    {
        movement = GetComponent<CharacterMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        
        // Если респаун точка не назначена, ищем по тегу
        if (respawnPoint == null)
        {
            GameObject respawnObj = GameObject.FindGameObjectWithTag("RespawnPoint");
            if (respawnObj != null)
                respawnPoint = respawnObj.transform;
        }
    }
    
    public void DieByMonster()
    {
        if (movement != null && movement.isDead) return;
        
        // Отмечаем как мёртвого
        if (movement != null)
            movement.isDead = true;
        
        // Останавливаем движение
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }
        
        // Отключаем коллайдер
        if (col != null)
            col.enabled = false;
        
        // Открепляемся от платформы если были приклеены
        if (movement != null)
        {
            // Используем рефлексию чтобы добраться до приватных полей
            var attachedPlatformField = typeof(CharacterMovement).GetField("attachedPlatform", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (attachedPlatformField != null)
                attachedPlatformField.SetValue(movement, null);
            
            var isHoldingField = typeof(CharacterMovement).GetField("isHolding", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (isHoldingField != null)
                isHoldingField.SetValue(movement, false);
            
            var currentBoatField = typeof(CharacterMovement).GetField("currentBoat", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (currentBoatField != null)
            {
                Boat boat = (Boat)currentBoatField.GetValue(movement);
                if (boat != null)
                    boat.SetPassenger(null);
                currentBoatField.SetValue(movement, null);
            }
        }
        
        // Запускаем анимацию смерти
        if (animator != null)
            animator.SetTrigger("Death");
        
        // Начинаем процесс возрождения
        StartCoroutine(RespawnCoroutine());
    }
    
    private IEnumerator RespawnCoroutine()
    {
        // Ждём анимацию смерти
        yield return new WaitForSeconds(deathAnimDuration);
        
        // Мигающий эффект возрождения
        float t = 0f;
        while (t < respawnDelay)
        {
            if (spriteRenderer != null)
                spriteRenderer.color = spriteRenderer.color.a > 0 ? Color.clear : Color.red;
            yield return new WaitForSeconds(0.12f);
            t += 0.12f;
        }
        
        // Возвращаем на точку респауна
        Vector3 respawnPos = respawnPoint != null ? respawnPoint.position : Vector3.zero;
        transform.position = new Vector3(respawnPos.x, respawnPos.y, 0f);
        
        // Восстанавливаем всё
        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = false;
        }
        
        if (col != null)
            col.enabled = true;
        
        if (movement != null)
            movement.isDead = false;
        
        
        // Возвращаем анимацию в Idle
        if (animator != null)
            animator.Play("Idle");
        
        // Сбрасываем всех монстров в сцене
        SleepingMonster[] monsters = FindObjectsOfType<SleepingMonster>();
        foreach (var monster in monsters)
        {
            monster.ResetMonster();
        }
        
        Boat boat = FindObjectOfType<Boat>();
        if (boat != null)
        {
            boat.RespawnBoat();
            Debug.Log("🚤 Лодка телепортирована после смерти козы!");
        }
        
        Debug.Log("Коза возродилась после смерти от монстра!");
    }
}