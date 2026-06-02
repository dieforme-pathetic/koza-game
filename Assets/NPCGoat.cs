using UnityEngine;

public class NPCGoat : MonoBehaviour
{
    [Header("Настройки")]
    public float detectionRadius = 3f;
    public bool showDebug = true;
    
    [Header("Анимация NPC")]
    public Animator npcAnimator;
    public string npcBoolName = "IsNear";
    
    [Header("Анимация игрока (меняется при обнаружении)")]
    public string playerBoolName = "IsNearNPC";  // Параметр в аниматоре игрока
    
    private Transform player;
    private Animator playerAnimator;
    private bool isPlayerNear = false;
    
    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerAnimator = playerObj.GetComponent<Animator>();
        }
        
        if (npcAnimator == null)
            npcAnimator = GetComponent<Animator>();
    }
    
    void Update()
    {
        if (player == null) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        bool near = distance <= detectionRadius;
        
        if (near != isPlayerNear)
        {
            isPlayerNear = near;
            
            // Меняем анимацию NPC
            if (npcAnimator != null)
                npcAnimator.SetBool(npcBoolName, isPlayerNear);
            
            // Меняем анимацию ИГРОКА
            if (playerAnimator != null)
                playerAnimator.SetBool(playerBoolName, isPlayerNear);
            
            if (showDebug)
                Debug.Log($"Игрок рядом: {isPlayerNear}");
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}