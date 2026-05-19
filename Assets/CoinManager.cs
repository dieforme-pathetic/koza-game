using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;
    private int coinCount = 0;

    void Awake()
    {
        Instance = this;
    }

    public void AddCoin()
    {
        coinCount++;
        Debug.Log("Монет собрано: " + coinCount);
    }
}