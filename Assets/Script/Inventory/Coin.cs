
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.CompareTag("Player"))
            return;

        if (GameManager.Instance == null)
        {
            Debug.LogError("No existe un GameManager en la escena.");
            return;
        }

        collected = true;

        GameManager.Instance.AddCoins(value);

        Destroy(gameObject);
    }
}
