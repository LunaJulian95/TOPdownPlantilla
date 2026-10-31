using UnityEngine;
using static Item;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;


    [Header("Equipamiento actual")]
    public Item currentWeapon;
    public Item currentArmor;
    public Item currentShield;
    private int appliedArmorHealthBonus = 0;


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
        Health health = GetPlayerHealth();

        if (health == null)
        {
            Debug.LogWarning("No se encontró Health del jugador.");
            return;
        }

        // Recuperar la vida base quitando
        // el bonus de la armadura anterior.
        int baseHealth = health.maxHealth - appliedArmorHealthBonus;

        // Obtener bonus de la nueva armadura.
        int newArmorBonus = 0;

        if (currentArmor != null)
        {
            newArmorBonus = currentArmor.healthBonus;
        }

        // Calcular diferencia entre armaduras.
        int bonusDifference = newArmorBonus - appliedArmorHealthBonus;

        // Aplicar nueva vida máxima.
        health.maxHealth = Mathf.Max(1, baseHealth + newArmorBonus);

        // Aumentar o reducir la vida actual según el cambio.
        health.currentHealth += bonusDifference;

        // Evitar valores fuera de rango.
        health.currentHealth = Mathf.Clamp(
            health.currentHealth,
            0,
            health.maxHealth
        );

        // Registrar el bonus aplicado.
        appliedArmorHealthBonus = newArmorBonus;

        Debug.Log(
            "========== VIDA RECALCULADA ==========" +
            "\nVida base: " + baseHealth +
            "\nBonus armadura: +" + newArmorBonus +
            "\nVida máxima: " + health.maxHealth +
            "\nVida actual: " + health.currentHealth +
            "\n======================================"
        );
    }
}