using UnityEngine;

public class PickupItem : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Предмет подобран!");
            Destroy(gameObject);
        }
    }
}
