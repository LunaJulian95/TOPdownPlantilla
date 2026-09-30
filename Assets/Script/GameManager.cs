using UnityEngine;
using TMPro; // Para mostrar texto

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int coins = 0;
    public TextMeshProUGUI coinsText; // Arrastra tu texto de UI acá

    void Awake()
    {
        Instance = this;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        coinsText.text = "Monedas: " + coins;
    }
}