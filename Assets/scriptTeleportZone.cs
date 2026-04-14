
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform teleportDestination;  // Точка выхода (перетащить в инспекторе)

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = teleportDestination.position;
        }
    }
}