
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public string mapBoundary;

    public List<InvetorySaveData> inventoryData;
    public int coins;
    public List<InvetorySaveData> hotbarSaveData;
    public List<ChestSaveData> chestSaveData;

    // Progreso de las misiones.
    public List<QuestProgressSaveData> questProgressData;
    public List<string> handinQuestIDs;

    public int playerLevel;
    public int playerXP;
    public int playerXPToNextLevel;

    public int playerMaxHealth;
    public int playerCurrentHealth;
    public int playerDamage;

    public int equippedWeaponID = -1;
    public int equippedWeaponSlot = -1;

    public int equippedArmorID = -1;
    public int equippedArmorSlot = -1;

    public int equippedShieldID = -1;
    public int equippedShieldSlot = -1;
}

[System.Serializable]
public class QuestProgressSaveData
{
    public string questID;
    public List<QuestObjectiveSaveData> objectives =
        new List<QuestObjectiveSaveData>();
}

[System.Serializable]
public class QuestObjectiveSaveData
{
    public string objectiveID;
    public int currentAmount;
}
