using UnityEngine;

public class Merchant : MonoBehaviour
{
    [Header("Objetos de este comerciante")]
    public ShopItem[] shopItems;

    public ShopItem GetShopItem(int itemID)
    {
        foreach (ShopItem shopItem in shopItems)
        {
            if (shopItem.itemPrefab == null)
                continue;

            Item item = shopItem.itemPrefab.GetComponent<Item>();

            if (item != null && item.ID == itemID)
            {
                return shopItem;
            }
        }

        return null;
    }

    public bool BuyItem(int itemID)
    {
        ShopItem shopItem = GetShopItem(itemID);

        if (shopItem == null)
        {
            Debug.LogWarning(
                "El comerciante no vende el item ID: " + itemID
            );

            return false;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("No existe GameManager.");
            return false;
        }

        if (InventoryController.Instance == null)
        {
            Debug.LogError("No existe InventoryController.");
            return false;
        }

        // Comprobar dinero
        if (GameManager.Instance.coins < shopItem.buyPrice)
        {
            Debug.Log("No tienes suficientes monedas.");
            return false;
        }

        // Primero intentamos meter el objeto
        bool added = InventoryController.Instance.AddItemByID(itemID);

        if (!added)
        {
            Debug.Log("No se pudo comprar. Inventario lleno.");
            return false;
        }

        // Solo cobramos si realmente se agregó
        GameManager.Instance.AddCoins(-shopItem.buyPrice);

        Debug.Log(
            "Compraste " +
            shopItem.itemPrefab.name +
            " por " +
            shopItem.buyPrice +
            " monedas."
        );

        return true;
    }

    public bool SellItem(GameObject itemObject)
    {
        if (itemObject == null)
            return false;

        Item item = itemObject.GetComponent<Item>();

        if (item == null)
        {
            Debug.LogWarning(
                "El objeto que intentas vender no tiene componente Item."
            );

            return false;
        }

        ShopItem shopItem = GetShopItem(item.ID);

        if (shopItem == null)
        {
            Debug.Log(
                "Este comerciante no compra: " +
                item.itemName
            );

            return false;
        }

        // Buscar el slot que contiene el objeto
        Slot itemSlot = null;

        foreach (Transform slotTransform in
                 InventoryController.Instance.inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null &&
                slot.currenItem == itemObject)
            {
                itemSlot = slot;
                break;
            }
        }

        if (itemSlot == null)
        {
            Debug.LogWarning(
                "No se encontró el objeto dentro del inventario."
            );

            return false;
        }

        // Limpiar el slot
        itemSlot.currenItem = null;

        // Dar monedas
        GameManager.Instance.AddCoins(shopItem.sellPrice);

        Debug.Log(
            "Vendiste " +
            item.itemName +
            " por " +
            shopItem.sellPrice +
            " monedas."
        );

        // Eliminar objeto
        Destroy(itemObject);

        return true;
    }
}