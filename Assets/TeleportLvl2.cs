using UnityEngine;

public class Teleport2 : MonoBehaviour
{
    [Header("Названия предметов для поиска")]
    public string item1Name = "Wrench";  // Имя в PickupItem.itemName
    public string item2Name = "Potion";
    
    [Header("Или теги (если не работает по имени)")]
    public string item1Tag = "";
    public string item2Tag = "";
    
    [Header("Настройки")]
    public Animator teleportAnimator;
    public string animatorBoolName = "TookItems";
    
    public ParticleSystem activationEffect;
    public AudioClip activationSound;
    
    private bool isActivated = false;
    private bool isPlayerNear = false;
    private GameObject foundItem1;
    private GameObject foundItem2;
    
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
        Debug.Log("=== ТЕЛЕПОРТ: проверка ===");
        
        // Ищем предметы в сцене
        foundItem1 = FindItem(item1Name, item1Tag);
        foundItem2 = FindItem(item2Name, item2Tag);
        
        bool hasItem1 = CheckItem(foundItem1);
        bool hasItem2 = CheckItem(foundItem2);
        
        Debug.Log($"Предмет 1 ({item1Name}): найдено={foundItem1 != null}, поднят={hasItem1}");
        Debug.Log($"Предмет 2 ({item2Name}): найдено={foundItem2 != null}, поднят={hasItem2}");
        
        if (hasItem1 && hasItem2)
        {
            ActivateTeleport();
        }
        else
        {
            if (!hasItem1) Debug.Log($"❌ НЕ ХВАТАЕТ: {item1Name}");
            if (!hasItem2) Debug.Log($"❌ НЕ ХВАТАЕТ: {item2Name}");
        }
    }
    
    GameObject FindItem(string itemName, string itemTag)
    {
        // Ищем по тегу
        if (!string.IsNullOrEmpty(itemTag))
        {
            GameObject tagged = GameObject.FindGameObjectWithTag(itemTag);
            if (tagged != null)
            {
                PickupItem p = tagged.GetComponent<PickupItem>();
                if (p != null) return tagged;
            }
        }
        
        // Ищем по имени в PickupItem
        PickupItem[] allItems = FindObjectsOfType<PickupItem>();
        foreach (PickupItem item in allItems)
        {
            if (item.GetItemName() == itemName)
                return item.gameObject;
        }
        
        return null;
    }
    
    bool CheckItem(GameObject item)
    {
        if (item == null) return false;
        PickupItem pickup = item.GetComponent<PickupItem>();
        if (pickup == null) return false;
        return pickup.IsPickedUp();
    }
    
    void ActivateTeleport()
    {
        isActivated = true;
        Debug.Log("✅ ТЕЛЕПОРТ АКТИВИРОВАН!");
        
        if (teleportAnimator != null)
            teleportAnimator.SetBool(animatorBoolName, true);
        
        if (activationEffect != null)
            activationEffect.Play();
        
        if (activationSound != null)
            AudioSource.PlayClipAtPoint(activationSound, transform.position);
        
        if (foundItem1 != null) Destroy(foundItem1);
        if (foundItem2 != null) Destroy(foundItem2);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            isPlayerNear = true;
            Debug.Log("🐐 Коза рядом с телепортом! Нажми E");
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
    
    public bool IsActivated()
    {
        return isActivated;
    }
}