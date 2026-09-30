using UnityEngine;
using TMPro;

public class PlayerLevel : MonoBehaviour
{
    [Header("Nivel")]
    public int level = 1;

    [Header("Experiencia")]
    public int currentXP = 0;
    public int xpToNextLevel = 100;

    [Header("UI")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;


    // =====================================================
    // DAR XP
    // =====================================================

    public void AddXP(int amount)
    {
        if (amount <= 0)
            return;


        currentXP += amount;


        Debug.Log(
            "XP recibida: " +
            amount +
            " | XP total: " +
            currentXP +
            "/" +
            xpToNextLevel
        );


        // Si recibió suficiente XP para varios niveles,
        // sube los niveles necesarios.
        while (
            currentXP >= xpToNextLevel &&
            xpToNextLevel > 0
        )
        {
            currentXP -= xpToNextLevel;

            LevelUp();
        }


        UpdateUI();
    }


    // =====================================================
    // SUBIR DE NIVEL
    // =====================================================

    private void LevelUp()
    {
        level++;


        xpToNextLevel =
            Mathf.RoundToInt(
                xpToNextLevel * 1.2f
            );


        // -------------------------------------------------
        // VIDA
        // -------------------------------------------------

        Health health =
            GetComponent<Health>();


        if (health != null)
        {
            health.maxHealth += 20;

            health.currentHealth += 20;


            if (
                health.currentHealth >
                health.maxHealth
            )
            {
                health.currentHealth =
                    health.maxHealth;
            }
        }


        // -------------------------------------------------
        // DAÑO
        // -------------------------------------------------

        PlayerCombat combat =
            GetComponent<PlayerCombat>();


        if (combat != null)
        {
            if (EquipmentManager.Instance != null)
            {
                EquipmentManager.Instance
                    .RecalculateDamage();
            }
            else
            {
                combat.baseDamage =
                    10 + ((level - 1) * 5);

                combat.damage =
                    combat.baseDamage;
            }
        }


        Debug.Log(
            "================================"
        );

        Debug.Log(
            "¡SUBISTE A NIVEL " +
            level +
            "!"
        );


        if (health != null)
        {
            Debug.Log(
                "Vida: " +
                health.currentHealth +
                "/" +
                health.maxHealth
            );
        }


        if (combat != null)
        {
            Debug.Log(
                "Daño: " +
                combat.damage
            );
        }


        Debug.Log(
            "================================"
        );
    }


    // =====================================================
    // ACTUALIZAR UI
    // =====================================================

    public void UpdateUI()
    {
        if (levelText != null)
        {
            levelText.text =
                "Nv: " + level;
        }


        if (xpText != null)
        {
            xpText.text =
                currentXP +
                " / " +
                xpToNextLevel;
        }
    }
}