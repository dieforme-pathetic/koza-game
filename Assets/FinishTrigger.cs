using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    public Animator goatAnimator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            goatAnimator.SetTrigger("Win");
        }
    }
}