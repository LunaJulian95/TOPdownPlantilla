using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public bool isPlayer = false;


    void Start()
    {
        // Solo poner la vida al máximo
        // si todavía no tiene una vida válida.
        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;
        }
    }


    public void TakeDamage(int amount)
    {
        // Protección del escudo
        if (
            isPlayer &&
            EquipmentManager.Instance != null &&
            EquipmentManager.Instance.currentShield != null
        )
        {
            int block =
                EquipmentManager.Instance.currentShield.blockBonus;

            int blockedAmount =
                (amount * block) / 100;

            amount -= blockedAmount;

            Debug.Log(
                $"Bloqueaste {blockedAmount} de daño! ({block}%)"
            );
        }


        currentHealth -= amount;


        Debug.Log(
            "Vida: " +
            currentHealth +
            "/" +
            maxHealth
        );


        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }


    void Die()
    {
        if (isPlayer)
        {
            Debug.Log("Jugador muerto");

            gameObject.SetActive(false);
        }
        else
        {
            LootDrop loot =
                GetComponent<LootDrop>();

            if (loot != null)
            {
                loot.DropLoot();
            }

            Destroy(gameObject);
        }
    }
}