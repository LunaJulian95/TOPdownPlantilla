using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;

    [Header("Paneles")]
    public GameObject inventoryPanel;

    public GameObject equipPanel;

    [Header("Slots")]
    public GameObject slotPrefab;

    public int slotCount = 20;
    public int equipSlotCount = 6;

    public int expansions = 0;

    [Header("Items")]
    public GameObject[] itemPrefabs;

    private ItemDiccionary itemDictionary;


    // =========================================================
    // AWAKE
    // =========================================================

    void Awake()
    {
        Instance = this;
    }


    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        itemDictionary =
            FindAnyObjectByType<ItemDiccionary>();

        CreateInventorySlots();
        CreateEquipSlots();
    }


    // =========================================================
    // CREAR SLOTS DEL INVENTARIO
    // =========================================================

    public void CreateInventorySlots()
    {
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        int totalSlots =
            slotCount +
            (8 * expansions);

        for (int i = 0; i < totalSlots; i++)
        {
            Instantiate(
                slotPrefab,
                inventoryPanel.transform
            );
        }
    }


    // =========================================================
    // CREAR SLOTS DE EQUIPAMIENTO
    // =========================================================

    public void CreateEquipSlots()
    {
        foreach (Transform child in equipPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < equipSlotCount; i++)
        {
            Instantiate(
                slotPrefab,
                equipPanel.transform
            );
        }
    }


    // =========================================================
    // AGREGAR ITEM AL INVENTARIO
    // =========================================================

    public bool AddItemByID(int id)
    {
        if (itemDictionary == null)
        {
            itemDictionary =
                FindAnyObjectByType<ItemDiccionary>();
        }

        if (itemDictionary == null)
        {
            Debug.LogError(
                "ItemDictionary no está asignado."
            );

            return false;
        }


        GameObject originalPrefab =
            itemDictionary.GetItemPrefab(id);


        if (originalPrefab == null)
        {
            Debug.LogError(
                "No existe item con ID: " +
                id
            );

            return false;
        }


        foreach (Transform slotTransform in
                 inventoryPanel.transform)
        {
            Slot slot =
                slotTransform.GetComponent<Slot>();


            if (slot != null &&
                slot.currenItem == null)
            {
                GameObject newItem =
                    Instantiate(
                        originalPrefab,
                        slot.transform
                    );


                RectTransform rect =
                    newItem.GetComponent<RectTransform>();


                if (rect != null)
                {
                    rect.anchoredPosition =
                        Vector2.zero;

                    rect.localScale =
                        Vector3.one;
                }


                slot.currenItem =
                    newItem;


                newItem.name =
                    originalPrefab.name;


                return true;
            }
        }


        Debug.Log("Inventario lleno.");

        return false;
    }


    // =========================================================
    // COMPATIBILIDAD
    // =========================================================

    public bool AddItem(GameObject itemPrefab)
    {
        if (itemPrefab == null)
            return false;


        Item item =
            itemPrefab.GetComponent<Item>();


        if (item != null)
        {
            return AddItemByID(item.ID);
        }


        return false;
    }


    // =========================================================
    // GUARDAR INVENTARIO
    // =========================================================

    public List<InvetorySaveData> GetInventoryItems()
    {
        List<InvetorySaveData> inventoryData =
            new List<InvetorySaveData>();


        foreach (Transform slotTransform in
                 inventoryPanel.transform)
        {
            Slot slot =
                slotTransform.GetComponent<Slot>();


            if (slot != null &&
                slot.currenItem != null)
            {
                Item item =
                    slot.currenItem.GetComponent<Item>();


                if (item != null)
                {
                    inventoryData.Add(
                        new InvetorySaveData
                        {
                            itemIDs =
                                item.ID,

                            slotIndex =
                                slotTransform.GetSiblingIndex()
                        }
                    );
                }
            }
        }


        return inventoryData;
    }


    // =========================================================
    // ELIMINAR ITEM
    // =========================================================

    public void RemoveItem(GameObject itemObject)
    {
        if (itemObject == null)
            return;


        // Buscar en inventario
        foreach (Transform slotTransform in
                 inventoryPanel.transform)
        {
            Slot slot =
                slotTransform.GetComponent<Slot>();


            if (slot != null &&
                slot.currenItem == itemObject)
            {
                slot.currenItem = null;

                break;
            }
        }


        // Buscar en equipamiento
        foreach (Transform slotTransform in
                 equipPanel.transform)
        {
            Slot slot =
                slotTransform.GetComponent<Slot>();


            if (slot != null &&
                slot.currenItem == itemObject)
            {
                slot.currenItem = null;

                break;
            }
        }
    }


    // =========================================================
    // CARGAR INVENTARIO
    // =========================================================

    public void SetInventoryItems(
        List<InvetorySaveData> inventoryData)
    {
        Debug.Log(
            "=== CARGANDO INVENTARIO ==="
        );


        if (inventoryData == null)
        {
            Debug.LogWarning(
                "inventoryData es NULL."
            );

            return;
        }


        if (itemDictionary == null)
        {
            itemDictionary =
                FindAnyObjectByType<ItemDiccionary>();
        }


        Debug.Log(
            "Items guardados: " +
            inventoryData.Count
        );


        Debug.Log(
            "Slots actuales: " +
            inventoryPanel.transform.childCount
        );


        int totalSlots =
            slotCount +
            (8 * expansions);


        foreach (InvetorySaveData saveData
                 in inventoryData)
        {
            Debug.Log(
                "Cargando ID: " +
                saveData.itemIDs +
                " en slot: " +
                saveData.slotIndex
            );


            if (saveData.slotIndex >= totalSlots)
            {
                Debug.LogWarning(
                    "El slot guardado no existe: " +
                    saveData.slotIndex
                );

                continue;
            }


            if (saveData.slotIndex >=
                inventoryPanel.transform.childCount)
            {
                Debug.LogWarning(
                    "No existe el hijo " +
                    saveData.slotIndex +
                    " en inventoryPanel."
                );

                continue;
            }


            Slot slot =
                inventoryPanel
                .transform
                .GetChild(saveData.slotIndex)
                .GetComponent<Slot>();


            if (slot == null)
            {
                Debug.LogWarning(
                    "El slot " +
                    saveData.slotIndex +
                    " no tiene componente Slot."
                );

                continue;
            }


            GameObject itemPrefab =
                itemDictionary.GetItemPrefab(
                    saveData.itemIDs
                );


            Debug.Log(
                "Prefab encontrado: " +
                itemPrefab
            );


            if (itemPrefab == null)
            {
                Debug.LogWarning(
                    "No existe prefab para ID: " +
                    saveData.itemIDs
                );

                continue;
            }


            GameObject item =
                Instantiate(
                    itemPrefab,
                    slot.transform
                );


            RectTransform rect =
                item.GetComponent<RectTransform>();


            if (rect != null)
            {
                rect.anchoredPosition =
                    Vector2.zero;

                rect.localScale =
                    Vector3.one;
            }


            item.name =
                itemPrefab.name;


            slot.currenItem =
                item;


            Debug.Log(
                "ITEM CARGADO CORRECTAMENTE: " +
                item.name
            );
        }
    }


    // =========================================================
    // LIMPIAR EQUIPAMIENTO
    // =========================================================

    public void ClearEquipSlots()
    {
        if (equipPanel == null)
            return;


        foreach (Transform slotTransform in
                 equipPanel.transform)
        {
            Slot slot =
                slotTransform.GetComponent<Slot>();


            if (slot == null)
                continue;


            if (slot.currenItem != null)
            {
                Destroy(
                    slot.currenItem
                );
            }


            slot.currenItem = null;
        }
    }


    // =========================================================
    // CARGAR ITEM EN SLOT DE EQUIPAMIENTO
    // =========================================================

    public GameObject LoadItemIntoEquipSlot(
        int itemID,
        int slotIndex)
    {
        if (itemDictionary == null)
        {
            itemDictionary =
                FindAnyObjectByType<ItemDiccionary>();
        }


        if (itemDictionary == null)
        {
            Debug.LogError(
                "No se encontró ItemDiccionary."
            );

            return null;
        }


        if (equipPanel == null)
        {
            Debug.LogError(
                "InventoryController: " +
                "equipPanel no está asignado."
            );

            return null;
        }


        if (slotIndex < 0 ||
            slotIndex >=
            equipPanel.transform.childCount)
        {
            Debug.LogWarning(
                "El slot de equipamiento no existe: " +
                slotIndex
            );

            return null;
        }


        GameObject itemPrefab =
            itemDictionary.GetItemPrefab(
                itemID
            );


        if (itemPrefab == null)
        {
            Debug.LogWarning(
                "No existe prefab para el item ID: " +
                itemID
            );

            return null;
        }


        Slot slot =
            equipPanel
            .transform
            .GetChild(slotIndex)
            .GetComponent<Slot>();


        if (slot == null)
        {
            Debug.LogWarning(
                "El slot de equipamiento no tiene " +
                "componente Slot."
            );

            return null;
        }


        if (slot.currenItem != null)
        {
            Destroy(
                slot.currenItem
            );

            slot.currenItem = null;
        }


        GameObject item =
            Instantiate(
                itemPrefab,
                slot.transform
            );


        RectTransform rect =
            item.GetComponent<RectTransform>();


        if (rect != null)
        {
            rect.anchoredPosition =
                Vector2.zero;

            rect.localScale =
                Vector3.one;
        }


        item.name =
            itemPrefab.name;


        slot.currenItem =
            item;


        Debug.Log(
            "Equipamiento cargado: " +
            item.name +
            " | Slot: " +
            slotIndex
        );


        return item;
    }
}