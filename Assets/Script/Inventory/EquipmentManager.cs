using UnityEngine;
using static Item;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;


    [Header("Equipamiento actual")]
    public Item currentWeapon;
    public Item currentArmor;
    public Item currentShield;


    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // =====================================================
    // BUSCAR VIDA DEL PLAYER
    // =====================================================

    private Health GetPlayerHealth()
    {
        Health[] all =
            FindObjectsOfType<Health>(true);


        foreach (Health h in all)
        {
            if (h.isPlayer)
                return h;
        }


        return null;
    }


    // =====================================================
    // EQUIPAR
    // =====================================================

    public void Equip(Item item)
    {
        if (item == null)
            return;


        // -------------------------------------------------
        // ARMA
        // -------------------------------------------------

        if (item.itemType == ItemType.Weapon)
        {
            if (currentWeapon == item)
                return;


            currentWeapon =
                item;


            RecalculateDamage();


            Debug.Log(
                "Arma equipada: " +
                item.itemName +
                " | Bonus: +" +
                item.damageBonus
            );
        }


        // -------------------------------------------------
        // ESCUDO
        // -------------------------------------------------

        else if (item.itemType == ItemType.Shield)
        {
            if (currentShield == item)
                return;


            currentShield =
                item;


            RecalculateDamage();


            Debug.Log(
                "Escudo equipado: " +
                item.itemName +
                " | Bonus daño: +" +
                item.damageBonusSecondary
            );
        }


        // -------------------------------------------------
        // ARMADURA
        // -------------------------------------------------

        else if (item.itemType == ItemType.Armor)
        {
            if (currentArmor == item)
                return;


            currentArmor =
                item;


            RecalculateHealth();


            Debug.Log(
                "Armadura equipada: " +
                item.itemName +
                " | Bonus vida: +" +
                item.healthBonus
            );
        }
    }


    // =====================================================
    // DESEQUIPAR
    // =====================================================

    public void Unequip(Item item)
    {
        if (item == null)
            return;


        if (item == currentWeapon)
        {
            currentWeapon = null;

            RecalculateDamage();
        }


        if (item == currentShield)
        {
            currentShield = null;

            RecalculateDamage();
        }


        if (item == currentArmor)
        {
            currentArmor = null;

            RecalculateHealth();
        }
    }


    // =====================================================
    // RECALCULAR DAÑO
    // =====================================================

    public void RecalculateDamage()
    {
        PlayerCombat combat =
            FindObjectOfType<PlayerCombat>(true);


        PlayerLevel playerLevel =
            FindObjectOfType<PlayerLevel>(true);


        if (combat == null)
        {
            Debug.LogWarning(
                "EquipmentManager: " +
                "no se encontró PlayerCombat."
            );

            return;
        }


        if (playerLevel == null)
        {
            Debug.LogWarning(
                "EquipmentManager: " +
                "no se encontró PlayerLevel."
            );

            return;
        }


        // =====================================================
        // DAÑO BASE SEGÚN NIVEL
        // =====================================================

        int calculatedBaseDamage =
            10 +
            ((playerLevel.level - 1) * 5);


        combat.baseDamage =
            calculatedBaseDamage;


        // =====================================================
        // BONUS DEL EQUIPAMIENTO
        // =====================================================

        int totalBonus = 0;


        // Arma
        if (currentWeapon != null)
        {
            totalBonus +=
                currentWeapon.damageBonus;
        }


        // Escudo
        if (currentShield != null)
        {
            totalBonus +=
                currentShield.damageBonusSecondary;
        }


        // =====================================================
        // DAÑO FINAL
        // =====================================================

        combat.damage =
            combat.baseDamage +
            totalBonus;


        Debug.Log(
            "================================"
        );


        Debug.Log(
            "DAÑO RECALCULADO"
        );


        Debug.Log(
            "Nivel: " +
            playerLevel.level
        );


        Debug.Log(
            "Daño base: " +
            combat.baseDamage
        );


        Debug.Log(
            "Bonus equipo: +" +
            totalBonus
        );


        Debug.Log(
            "Daño final: " +
            combat.damage
        );


        Debug.Log(
            "================================"
        );
    }


    // =====================================================
    // RECALCULAR VIDA
    // =====================================================

    public void RecalculateHealth()
    {
        Health health =
            GetPlayerHealth();


        if (health == null)
            return;


        int baseHealth =
            health.maxHealth;


        // Quitar bonus de armadura anterior
        if (currentArmor != null)
        {
            baseHealth -=
                currentArmor.healthBonus;
        }


        if (baseHealth < 1)
        {
            baseHealth = 1;
        }


        int oldMaxHealth =
            health.maxHealth;


        health.maxHealth =
            baseHealth;


        // Aplicar nueva armadura
        if (currentArmor != null)
        {
            health.maxHealth +=
                currentArmor.healthBonus;
        }


        // Mantener proporción de vida
        if (oldMaxHealth > 0)
        {
            float healthPercent =
                (float)health.currentHealth /
                oldMaxHealth;


            health.currentHealth =
                Mathf.RoundToInt(
                    health.maxHealth *
                    healthPercent
                );
        }


        if (health.currentHealth >
            health.maxHealth)
        {
            health.currentHealth =
                health.maxHealth;
        }


        Debug.Log(
            "Vida recalculada: " +
            health.currentHealth +
            "/" +
            health.maxHealth
        );
    }
}