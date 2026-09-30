using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    // =====================================================
    // PLAYER
    // =====================================================

    public Vector3 playerPosition;

    // Área actual del mapa
    public string mapBoundary;


    // =====================================================
    // INVENTARIO
    // =====================================================

    public List<InvetorySaveData> inventoryData;


    // =====================================================
    // HOTBAR
    // =====================================================

    public List<InvetorySaveData> hotbarSaveData;


    // =====================================================
    // COFRES
    // =====================================================

    public List<ChestSaveData> chestSaveData;


    // =====================================================
    // NIVEL Y EXPERIENCIA
    // =====================================================

    public int playerLevel;
    public int playerXP;
    public int playerXPToNextLevel;


    // =====================================================
    // VIDA Y DAÑO
    // =====================================================

    public int playerMaxHealth;
    public int playerCurrentHealth;
    public int playerDamage;


    // =====================================================
    // EQUIPAMIENTO
    // =====================================================

    // Arma
    public int equippedWeaponID = -1;
    public int equippedWeaponSlot = -1;

    // Armadura
    public int equippedArmorID = -1;
    public int equippedArmorSlot = -1;

    // Escudo
    public int equippedShieldID = -1;
    public int equippedShieldSlot = -1;
}