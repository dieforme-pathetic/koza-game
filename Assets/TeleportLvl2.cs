using UnityEngine;

public class Teleport2 : MonoBehaviour
{
    [Header("Предметы для активации (перетащи из сцены)")]
    public GameObject requiredItem1;
    public GameObject requiredItem2;
    
    [Header("Настройки")]
    public GameObject interactionPrompt;
    public Animator teleportAnimator;
    public string animatorBoolName = "TookItems";
    
    [Header("Эффекты")]
    public ParticleSystem activationEffect;
    public AudioClip activationSound;
    
    private bool isActivated = false;
    private bool isPlayerNear = false;
    
    void Start()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        
        // Проверяем наличие предметов при старте
        if (requiredItem1 == null)
            Debug.LogWarning("⚠️ requiredItem1 не назначен!");
        if (requiredItem2 == null)
            Debug.LogWarning("⚠️ requiredItem2 не назначен!");
        if (teleportAnimator == null)
            Debug.LogWarning("⚠️ teleportAnimator не назначен! (перетащи объект с Animator)");
    }
    
    void Update()
    {
        if (isActivated) return;
        
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            TryActivate();
        }
    }
    
    void TryActivate()
    {
        bool hasItem1 = false;
        bool hasItem2 = false;
        
        Debug.Log("🔍 Проверка предметов...");
        
        // Проверяем первый предмет
        if (requiredItem1 != null)
        {
            PickupItem item1 = requiredItem1.GetComponent<PickupItem>();
            if (item1 != null)
            {
                hasItem1 = item1.IsPickedUp();
                Debug.Log($"Предмет 1: {requiredItem1.name}, поднят: {hasItem1}");
            }
            else
            {
                Debug.LogWarning($"⚠️ На {requiredItem1.name} нет компонента PickupItem!");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ requiredItem1 не назначен!");
        }
        
        // Проверяем второй предмет
        if (requiredItem2 != null)
        {
            PickupItem item2 = requiredItem2.GetComponent<PickupItem>();
            if (item2 != null)
            {
                hasItem2 = item2.IsPickedUp();
                Debug.Log($"Предмет 2: {requiredItem2.name}, поднят: {hasItem2}");
            }
            else
            {
                Debug.LogWarning($"⚠️ На {requiredItem2.name} нет компонента PickupItem!");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ requiredItem2 не назначен!");
        }
        
        if (hasItem1 && hasItem2)
        {
            ActivateTeleport();
        }
        else
        {
            Debug.Log($"❌ Телепорт не активирован! Нужно принести оба предмета! (1:{hasItem1}, 2:{hasItem2})");
        }
    }
    
    void ActivateTeleport()
    {
        isActivated = true;
        
        Debug.Log("✅ ТЕЛЕПОРТ АКТИВИРОВАН!");
        
        // Включаем анимацию
        if (teleportAnimator != null)
        {
            teleportAnimator.SetBool(animatorBoolName, true);
            Debug.Log($"🎬 Аниматор: {animatorBoolName} = true");
        }
        else
        {
            Debug.LogWarning("⚠️ teleportAnimator не назначен! Анимация не будет работать.");
        }
        
        // Эффекты
        if (activationEffect != null)
        {
            activationEffect.Play();
            Debug.Log("✨ Эффект активации!");
        }
        
        if (activationSound != null)
        {
            AudioSource.PlayClipAtPoint(activationSound, transform.position);
            Debug.Log("🔊 Звук активации!");
        }
        
        // Удаляем предметы (только если они подняты)
        if (requiredItem1 != null)
        {
            PickupItem item1 = requiredItem1.GetComponent<PickupItem>();
            if (item1 != null && item1.IsPickedUp())
            {
                Destroy(requiredItem1);
                Debug.Log($"🗑️ Предмет {requiredItem1.name} удалён");
            }
        }
        
        if (requiredItem2 != null)
        {
            PickupItem item2 = requiredItem2.GetComponent<PickupItem>();
            if (item2 != null && item2.IsPickedUp())
            {
                Destroy(requiredItem2);
                Debug.Log($"🗑️ Предмет {requiredItem2.name} удалён");
            }
        }
        
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            isPlayerNear = true;
            
            if (interactionPrompt != null)
                interactionPrompt.SetActive(true);
            
            Debug.Log("🐐 Коза рядом с телепортом! Нажми E, если есть оба предмета.");
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);
        }
    }
    
    public bool IsActivated()
    {
        return isActivated;
    }
}