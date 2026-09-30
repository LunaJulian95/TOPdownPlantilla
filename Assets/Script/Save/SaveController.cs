using System.IO;
using NUnit.Framework;
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


        Debug.Log(
            "ARCHIVO DE GUARDADO: " +
            saveLocation
        );


        inventoryController =
            FindAnyObjectByType<
                InventoryController
            >();


        hotbarController =
            FindAnyObjectByType<
                HotBarController
            >();


        playerLevel =
            FindAnyObjectByType<
                PlayerLevel
            >();


        equipmentManager =
            FindAnyObjectByType<
                EquipmentManager
            >();


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
            Debug.LogError(
                "NO SE ENCONTRÓ EL PLAYER."
            );

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
            Debug.LogError(
                "El Player no tiene componente Health."
            );

            return;
        }


        if (combat == null)
        {
            Debug.LogError(
                "El Player no tiene componente PlayerCombat."
            );

            return;
        }


        // -----------------------------------------------------
        // ASEGURAR CONTROLLERS
        // -----------------------------------------------------

        if (inventoryController == null)
        {
            inventoryController =
                FindAnyObjectByType<
                    InventoryController
                >();
        }


        if (hotbarController == null)
        {
            hotbarController =
                FindAnyObjectByType<
                    HotBarController
                >();
        }


        if (playerLevel == null)
        {
            playerLevel =
                FindAnyObjectByType<
                    PlayerLevel
                >();
        }


        if (equipmentManager == null)
        {
            equipmentManager =
                FindAnyObjectByType<
                    EquipmentManager
                >();
        }


        // -----------------------------------------------------
        // COMPROBAR CONTROLLERS
        // -----------------------------------------------------

        if (inventoryController == null)
        {
            Debug.LogError(
                "NO SE ENCONTRÓ InventoryController."
            );

            return;
        }


        if (hotbarController == null)
        {
            Debug.LogError(
                "NO SE ENCONTRÓ HotBarController."
            );

            return;
        }


        if (playerLevel == null)
        {
            Debug.LogError(
                "NO SE ENCONTRÓ PlayerLevel."
            );

            return;
        }


        // -----------------------------------------------------
        // OBTENER ITEMS
        // -----------------------------------------------------

        var inventoryItems =
            inventoryController
            .GetInventoryItems();


        var hotbarItems =
            hotbarController
            .GetHotBarItems();


        Debug.Log(
            "Items inventario encontrados: " +
            inventoryItems.Count
        );


        Debug.Log(
            "Items hotbar encontrados: " +
            hotbarItems.Count
        );


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
                    continue;


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


        Debug.Log(
            "Arma equipada ID: " +
            equippedWeaponID
        );


        Debug.Log(
            "Armadura equipada ID: " +
            equippedArmorID
        );


        Debug.Log(
            "Escudo equipado ID: " +
            equippedShieldID
        );


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


        // -----------------------------------------------------
        // INFORMACIÓN
        // -----------------------------------------------------

        Debug.Log(
            "=============================="
        );


        Debug.Log(
            "=== PARTIDA GUARDADA ==="
        );


        Debug.Log(
            "Nivel: " +
            data.playerLevel
        );


        Debug.Log(
            "XP: " +
            data.playerXP +
            "/" +
            data.playerXPToNextLevel
        );


        Debug.Log(
            "Vida: " +
            data.playerCurrentHealth +
            "/" +
            data.playerMaxHealth
        );


        Debug.Log(
            "Daño: " +
            data.playerDamage
        );


        Debug.Log(
            "Items inventario: " +
            data.inventoryData.Count
        );


        Debug.Log(
            "Items hotbar: " +
            data.hotbarSaveData.Count
        );


        Debug.Log(
            "Arma equipada: ID " +
            data.equippedWeaponID
        );


        Debug.Log(
            "Armadura equipada: ID " +
            data.equippedArmorID
        );


        Debug.Log(
            "Escudo equipado: ID " +
            data.equippedShieldID
        );


        Debug.Log(
            "Área: " +
            data.mapBoundary
        );


        Debug.Log(
            "Archivo: " +
            saveLocation
        );


        Debug.Log(
            "=============================="
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
            return chestsSaveData;


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
            Debug.Log(
                "No existe partida guardada."
            );

            return;
        }


        Debug.Log(
            "=============================="
        );


        Debug.Log(
            "=== CARGANDO PARTIDA ==="
        );


        Debug.Log(
            "=============================="
        );


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
            Debug.LogError(
                "No se pudo cargar SaveData."
            );

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


            Debug.Log(
                "Inventario cargado: " +
                data.inventoryData.Count
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


            Debug.Log(
                "Hotbar cargada: " +
                data.hotbarSaveData.Count
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


                Debug.Log(
                    "Vida cargada: " +
                    health.currentHealth +
                    "/" +
                    health.maxHealth
                );
            }


            if (combat != null)
            {
                combat.damage =
                    data.playerDamage;


                Debug.Log(
                    "Daño cargado: " +
                    combat.damage
                );
            }
        }


        Debug.Log(
            "=============================="
        );


        Debug.Log(
            "=== PARTIDA CARGADA ==="
        );


        Debug.Log(
            "=============================="
        );
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
            Debug.LogWarning(
                "No se encontró EquipmentManager."
            );

            return;
        }


        if (inventoryController == null)
        {
            Debug.LogWarning(
                "No se encontró InventoryController."
            );

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


        // -----------------------------------------------------
        // INFORMACIÓN
        // -----------------------------------------------------

        Debug.Log(
            "=== EQUIPAMIENTO CARGADO ==="
        );


        Debug.Log(
            "Arma ID: " +
            data.equippedWeaponID
        );


        Debug.Log(
            "Armadura ID: " +
            data.equippedArmorID
        );


        Debug.Log(
            "Escudo ID: " +
            data.equippedShieldID
        );
    }


    // =========================================================
    // CARGAR ESTADO DE COFRES
    // =========================================================

    private void LoadChestsState(
        List<ChestSaveData> chestsSaveData)
    {
        if (chestsSaveData == null)
            return;


        if (chests == null)
            return;


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