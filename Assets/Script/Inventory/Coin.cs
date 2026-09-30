using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Acá sumas al GameManager
            GameManager.Instance.AddCoins(value);
            Destroy(gameObject);
        }
    }
}