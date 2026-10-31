using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Monedas")]
    public int coins = 0;

    [Header("Interfaz")]
    public TextMeshProUGUI coinsText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        UpdateCoinsUI();
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;

        coins += amount;
        UpdateCoinsUI();

        Debug.Log("Monedas actuales: " + coins);
    }

    public bool SpendCoins(int amount)
    {
        if (amount <= 0 || coins < amount)
            return false;

        coins -= amount;
        UpdateCoinsUI();

        return true;
    }

    private void UpdateCoinsUI()
    {
        if (coinsText != null)
            coinsText.text = "Monedas: " + coins;
    }
}