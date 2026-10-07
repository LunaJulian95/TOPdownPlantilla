using System.IO;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SaveController : MonoBehaviour
{
    private string saveLocation;

    private InventoryController inventoryController;
    private HotBarController hotbarController;
    private PlayerLevel playerLevel;
    private Chest[] chests;
    private EquipmentManager equipmentManager;


    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        InitializeComponents();

        LoadGame();
    }


    // =========================================================
    // INICIALIZAR COMPONENTES
    // =========================================================

    private void InitializeComponents()
    {
        saveLocation =
            Path.Combine(
                Application.persistentDataPath,
                "saveData.json"
            );

        inventoryController =
            FindAnyObjectByType<InventoryController>();

        hotbarController =
            FindAnyObjectByType<HotBarController>();

        playerLevel =
            FindAnyObjectByType<PlayerLevel>();

        equipmentManager =
            FindAnyObjectByType<EquipmentManager>();

        chests =
            FindObjectsByType<Chest>(
                FindObjectsSortMode.None
            );
    }


    // =========================================================
    // GUARDAR
    // =========================================================

    [System.Obsolete]
    public void SaveGame()
    {
        // -----------------------------------------------------
        // PLAYER
        // -----------------------------------------------------

        GameObject player =
            GameObject.FindWithTag("Player");

        if (player == null)
        {
            return;
        }


        // -----------------------------------------------------
        // COMPONENTES PLAYER
        // -----------------------------------------------------

        Health health =
            player.GetComponent<Health>();

        PlayerCombat combat =
            player.GetComponent<PlayerCombat>();

        if (health == null)
        {
            return;
        }

        if (combat == null)
        {
            return;
        }


        // -----------------------------------------------------
        // ASEGURAR CONTROLLERS
        // -----------------------------------------------------

        if (inventoryController == null)
        {
            inventoryController =
                FindAnyObjectByType<InventoryController>();
        }

        if (hotbarController == null)
        {
            hotbarController =
                FindAnyObjectByType<HotBarController>();
        }

        if (playerLevel == null)
        {
            playerLevel =
                FindAnyObjectByType<PlayerLevel>();
        }

        if (equipmentManager == null)
        {
            equipmentManager =
                FindAnyObjectByType<EquipmentManager>();
        }


        // -----------------------------------------------------
        // COMPROBAR CONTROLLERS
        // -----------------------------------------------------

        if (inventoryController == null)
        {
            return;
        }

        if (hotbarController == null)
        {
            return;
        }

        if (playerLevel == null)
        {
            return;
        }


        // -----------------------------------------------------
        // OBTENER ITEMS
        // -----------------------------------------------------

        var inventoryItems =
            inventoryController.GetInventoryItems();

        var hotbarItems =
            hotbarController.GetHotBarItems();


        // -----------------------------------------------------
        // OBTENER EQUIPAMIENTO
        // -----------------------------------------------------

        int equippedWeaponID = -1;
        int equippedWeaponSlot = -1;

        int equippedArmorID = -1;
        int equippedArmorSlot = -1;

        int equippedShieldID = -1;
        int equippedShieldSlot = -1;


        if (equipmentManager != null &&
            inventoryController.equipPanel != null)
        {
            Transform equipPanel =
                inventoryController
                .equipPanel
                .transform;

            for (int i = 0;
                 i < equipPanel.childCount;
                 i++)
            {
                Slot slot =
                    equipPanel
                    .GetChild(i)
                    .GetComponent<Slot>();

                if (slot == null ||
                    slot.currenItem == null)
                {
                    continue;
                }

                Item item =
                    slot.currenItem
                    .GetComponent<Item>();

                if (item == null)
                {
                    continue;
                }


                // -------------------------------------------------
                // ARMA
                // -------------------------------------------------

                if (item ==
                    equipmentManager.currentWeapon)
                {
                    equippedWeaponID =
                        item.ID;

                    equippedWeaponSlot =
                        i;
                }


                // -------------------------------------------------
                // ARMADURA
                // -------------------------------------------------

                if (item ==
                    equipmentManager.currentArmor)
                {
                    equippedArmorID =
                        item.ID;

                    equippedArmorSlot =
                        i;
                }


                // -------------------------------------------------
                // ESCUDO
                // -------------------------------------------------

                if (item ==
                    equipmentManager.currentShield)
                {
                    equippedShieldID =
                        item.ID;

                    equippedShieldSlot =
                        i;
                }
            }
        }


        // -----------------------------------------------------
        // ÁREA DEL MAPA
        // -----------------------------------------------------

        string mapBoundary = "";

        CinemachineConfiner2D confiner =
            FindAnyObjectByType<
                CinemachineConfiner2D
            >();

        if (confiner != null &&
            confiner.BoundingShape2D != null)
        {
            mapBoundary =
                confiner
                .BoundingShape2D
                .gameObject
                .name;
        }


        // -----------------------------------------------------
        // CREAR DATOS
        // -----------------------------------------------------

        SaveData data =
            new SaveData
            {
                // Player
                playerPosition =
                    player.transform.position,

                // Mapa
                mapBoundary =
                    mapBoundary,

                // Inventario
                inventoryData =
                    inventoryItems,

                // Hotbar
                hotbarSaveData =
                    hotbarItems,

                // Nivel
                playerLevel =
                    playerLevel.level,

                // XP
                playerXP =
                    playerLevel.currentXP,

                // XP necesaria
                playerXPToNextLevel =
                    playerLevel.xpToNextLevel,

                // Vida máxima
                playerMaxHealth =
                    health.maxHealth,

                // Vida actual
                playerCurrentHealth =
                    health.currentHealth,

                // Daño
                playerDamage =
                    combat.damage,

                // Coins
                coins =
                    GameManager.Instance != null
                    ? GameManager.Instance.coins
                    : 0,

                // -------------------------------------------------
                // EQUIPAMIENTO
                // -------------------------------------------------

                equippedWeaponID =
                    equippedWeaponID,

                equippedWeaponSlot =
                    equippedWeaponSlot,

                equippedArmorID =
                    equippedArmorID,

                equippedArmorSlot =
                    equippedArmorSlot,

                equippedShieldID =
                    equippedShieldID,

                equippedShieldSlot =
                    equippedShieldSlot,

                // Cofres
                chestSaveData =
                    GetChestsState()
            };


        // -----------------------------------------------------
        // GUARDAR JSON
        // -----------------------------------------------------

        string json =
            JsonUtility.ToJson(
                data,
                true
            );

        File.WriteAllText(
            saveLocation,
            json
        );
    }


    // =========================================================
    // GUARDAR ESTADO DE COFRES
    // =========================================================

    private List<ChestSaveData> GetChestsState()
    {
        List<ChestSaveData> chestsSaveData =
            new List<ChestSaveData>();

        if (chests == null)
        {
            return chestsSaveData;
        }

        foreach (Chest chest in chests)
        {
            ChestSaveData chestSaveData =
                new ChestSaveData
                {
                    chestID =
                        chest.ChestID,

                    isOpened =
                        chest.IsOpened
                };

            chestsSaveData.Add(
                chestSaveData
            );
        }

        return chestsSaveData;
    }


    // =========================================================
    // CARGAR
    // =========================================================

    [System.Obsolete]
    public void LoadGame()
    {
        if (!File.Exists(saveLocation))
        {
            return;
        }


        // -----------------------------------------------------
        // LEER JSON
        // -----------------------------------------------------

        string json =
            File.ReadAllText(
                saveLocation
            );

        SaveData data =
            JsonUtility.FromJson<SaveData>(
                json
            );

        if (data == null)
        {
            return;
        }


        // -----------------------------------------------------
        // PLAYER
        // -----------------------------------------------------

        GameObject player =
            GameObject.FindWithTag("Player");

        if (player != null)
        {
            player.transform.position =
                data.playerPosition;
        }


        // -----------------------------------------------------
        // MAPA
        // -----------------------------------------------------

        if (!string.IsNullOrEmpty(
            data.mapBoundary))
        {
            GameObject areaObject =
                GameObject.Find(
                    data.mapBoundary
                );

            if (areaObject != null)
            {
                PolygonCollider2D mapBoundary =
                    areaObject.GetComponent<
                        PolygonCollider2D
                    >();

                CinemachineConfiner2D confiner =
                    FindAnyObjectByType<
                        CinemachineConfiner2D
                    >();

                if (confiner != null &&
                    mapBoundary != null)
                {
                    confiner.BoundingShape2D =
                        mapBoundary;
                }

                if (MapControllerDynamic.Instance != null)
                {
                    MapControllerDynamic.Instance
                        .GenerateMap(
                            mapBoundary
                        );

                    if (SoundMusicManager.Instance != null)
                    {
                        SoundMusicManager.Instance
                            .PlayMusicWithCrossFade(
                                mapBoundary.name
                            );
                    }
                }
                else
                {
                    if (SoundMusicManager.Instance != null)
                    {
                        SoundMusicManager.Instance
                            .PlayMusicWithCrossFade(
                                "T1"
                            );
                    }
                }
            }
        }


        // -----------------------------------------------------
        // INVENTARIO
        // -----------------------------------------------------

        if (inventoryController != null &&
            data.inventoryData != null)
        {
            inventoryController
                .SetInventoryItems(
                    data.inventoryData
                );
        }


        // -----------------------------------------------------
        // COFRES
        // -----------------------------------------------------

        if (data.chestSaveData != null)
        {
            LoadChestsState(
                data.chestSaveData
            );
        }


        // -----------------------------------------------------
        // HOTBAR
        // -----------------------------------------------------

        if (hotbarController != null &&
            data.hotbarSaveData != null)
        {
            hotbarController
                .SetHotBarItems(
                    data.hotbarSaveData
                );
        }


        // -----------------------------------------------------
        // NIVEL Y XP
        // -----------------------------------------------------

        if (playerLevel != null)
        {
            playerLevel.level =
                data.playerLevel;

            playerLevel.currentXP =
                data.playerXP;

            playerLevel.xpToNextLevel =
                data.playerXPToNextLevel;

            playerLevel.UpdateUI();
        }


        // -----------------------------------------------------
        // COINS
        // -----------------------------------------------------

        if (GameManager.Instance != null)
        {
            GameManager.Instance.coins =
                data.coins;

            if (GameManager.Instance.coinsText != null)
            {
                GameManager.Instance.coinsText.text =
                    "Monedas: " +
                    GameManager.Instance.coins;
            }
        }


        // -----------------------------------------------------
        // EQUIPAMIENTO
        // -----------------------------------------------------

        LoadEquipment(data);


        // -----------------------------------------------------
        // VIDA Y DAÑO
        // -----------------------------------------------------

        if (player != null)
        {
            Health health =
                player.GetComponent<Health>();

            PlayerCombat combat =
                player.GetComponent<PlayerCombat>();

            if (health != null)
            {
                health.maxHealth =
                    data.playerMaxHealth;

                health.currentHealth =
                    data.playerCurrentHealth;
            }

            if (combat != null)
            {
                combat.damage =
                    data.playerDamage;
            }
        }
    }


    // =========================================================
    // CARGAR EQUIPAMIENTO
    // =========================================================

    private void LoadEquipment(
        SaveData data)
    {
        if (equipmentManager == null)
        {
            equipmentManager =
                FindAnyObjectByType<
                    EquipmentManager
                >();
        }

        if (inventoryController == null)
        {
            inventoryController =
                FindAnyObjectByType<
                    InventoryController
                >();
        }

        if (equipmentManager == null)
        {
            return;
        }

        if (inventoryController == null)
        {
            return;
        }


        // -----------------------------------------------------
        // LIMPIAR REFERENCIAS ACTUALES
        // -----------------------------------------------------

        equipmentManager.currentWeapon =
            null;

        equipmentManager.currentArmor =
            null;

        equipmentManager.currentShield =
            null;


        // -----------------------------------------------------
        // LIMPIAR SLOTS
        // -----------------------------------------------------

        inventoryController
            .ClearEquipSlots();


        // -----------------------------------------------------
        // ARMA
        // -----------------------------------------------------

        if (data.equippedWeaponID >= 0 &&
            data.equippedWeaponSlot >= 0)
        {
            GameObject weapon =
                inventoryController
                .LoadItemIntoEquipSlot(
                    data.equippedWeaponID,
                    data.equippedWeaponSlot
                );

            if (weapon != null)
            {
                Item item =
                    weapon.GetComponent<Item>();

                if (item != null)
                {
                    equipmentManager.Equip(
                        item
                    );
                }
            }
        }


        // -----------------------------------------------------
        // ARMADURA
        // -----------------------------------------------------

        if (data.equippedArmorID >= 0 &&
            data.equippedArmorSlot >= 0)
        {
            GameObject armor =
                inventoryController
                .LoadItemIntoEquipSlot(
                    data.equippedArmorID,
                    data.equippedArmorSlot
                );

            if (armor != null)
            {
                Item item =
                    armor.GetComponent<Item>();

                if (item != null)
                {
                    equipmentManager.Equip(
                        item
                    );
                }
            }
        }


        // -----------------------------------------------------
        // ESCUDO
        // -----------------------------------------------------

        if (data.equippedShieldID >= 0 &&
            data.equippedShieldSlot >= 0)
        {
            GameObject shield =
                inventoryController
                .LoadItemIntoEquipSlot(
                    data.equippedShieldID,
                    data.equippedShieldSlot
                );

            if (shield != null)
            {
                Item item =
                    shield.GetComponent<Item>();

                if (item != null)
                {
                    equipmentManager.Equip(
                        item
                    );
                }
            }
        }
    }


    // =========================================================
    // CARGAR ESTADO DE COFRES
    // =========================================================

    private void LoadChestsState(
        List<ChestSaveData> chestsSaveData)
    {
        if (chestsSaveData == null)
        {
            return;
        }

        if (chests == null)
        {
            return;
        }

        foreach (Chest chest in chests)
        {
            ChestSaveData chestSaveData =
                chestsSaveData.FirstOrDefault(
                    c =>
                        c.chestID ==
                        chest.ChestID
                );

            if (chestSaveData != null)
            {
                chest.SetOpened(
                    chestSaveData.isOpened
                );
            }
        }
    }
}