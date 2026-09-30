using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance;

    [Header("Panel principal")]
    [SerializeField] private GameObject shopPanel;

    [Header("Paneles")]
    [SerializeField] private GameObject buyPanel;
    [SerializeField] private GameObject sellPanel;

    [Header("Contenedores")]
    [SerializeField] private Transform buyContainer;
    [SerializeField] private Transform sellContainer;

    [Header("Prefabs")]
    [SerializeField] private GameObject shopItemButtonPrefab;

    [Header("Información")]
    [SerializeField] private TMP_Text coinsText;

    private Merchant currentMerchant;

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

        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    // =========================================================
    // ABRIR TIENDA
    // =========================================================

    public void OpenShop(Merchant merchant)
    {
        if (merchant == null)
        {
            Debug.LogWarning("No se encontró Merchant.");
            return;
        }

        currentMerchant = merchant;

        shopPanel.SetActive(true);

        ShowBuyPanel();

        UpdateCoinsText();

        CreateBuyItems();
    }

    // =========================================================
    // CERRAR TIENDA
    // =========================================================

    public void CloseShop()
    {
        currentMerchant = null;

        ClearContainer(buyContainer);
        ClearContainer(sellContainer);

        shopPanel.SetActive(false);

        // Reactivar el juego
        PauseController.SetPause(false);
    }

    // =========================================================
    // PANEL COMPRAR
    // =========================================================

    public void ShowBuyPanel()
    {
        buyPanel.SetActive(true);
        sellPanel.SetActive(false);

        CreateBuyItems();
    }

    // =========================================================
    // PANEL VENDER
    // =========================================================

    public void ShowSellPanel()
    {
        buyPanel.SetActive(false);
        sellPanel.SetActive(true);

        CreateSellItems();
    }

    // =========================================================
    // CREAR OBJETOS PARA COMPRAR
    // =========================================================

    private void CreateBuyItems()
    {
        ClearContainer(buyContainer);

        if (currentMerchant == null)
            return;

        foreach (ShopItem shopItem in currentMerchant.shopItems)
        {
            if (shopItem == null)
                continue;

            if (shopItem.itemPrefab == null)
                continue;

            Item item = shopItem.itemPrefab.GetComponent<Item>();

            if (item == null)
                continue;

            // -1 significa que el comerciante NO vende este objeto
            if (shopItem.buyPrice < 0)
                continue;

            GameObject buttonObject =
                Instantiate(
                    shopItemButtonPrefab,
                    buyContainer
                );

            TMP_Text text =
                buttonObject.GetComponentInChildren<TMP_Text>();

            if (text != null)
            {
                text.text =
                    item.itemName +
                    "\nComprar: " +
                    shopItem.buyPrice +
                    " monedas";
            }

            Button button =
                buttonObject.GetComponent<Button>();

            if (button != null)
            {
                int itemID = item.ID;

                button.onClick.AddListener(
                    () => BuyItem(itemID)
                );
            }
        }
    }

    // =========================================================
    // COMPRAR
    // =========================================================

    private void BuyItem(int itemID)
    {
        if (currentMerchant == null)
            return;

        bool success =
            currentMerchant.BuyItem(itemID);

        if (success)
        {
            UpdateCoinsText();
        }

        // Actualizamos por si cambió el inventario
        CreateBuyItems();
    }

    // =========================================================
    // CREAR OBJETOS PARA VENDER
    // =========================================================

    private void CreateSellItems()
    {
        ClearContainer(sellContainer);

        if (currentMerchant == null)
            return;

        if (InventoryController.Instance == null)
            return;

        Transform inventory =
            InventoryController.Instance.inventoryPanel.transform;

        foreach (Transform slotTransform in inventory)
        {
            Slot slot =
                slotTransform.GetComponent<Slot>();

            if (slot == null)
                continue;

            if (slot.currenItem == null)
                continue;

            Item item =
                slot.currenItem.GetComponent<Item>();

            if (item == null)
                continue;

            ShopItem shopItem =
                currentMerchant.GetShopItem(item.ID);

            // El comerciante no compra este objeto
            if (shopItem == null)
                continue;

            // -1 significa que no compra
            if (shopItem.sellPrice < 0)
                continue;

            GameObject buttonObject =
                Instantiate(
                    shopItemButtonPrefab,
                    sellContainer
                );

            TMP_Text text =
                buttonObject.GetComponentInChildren<TMP_Text>();

            if (text != null)
            {
                text.text =
                    item.itemName +
                    "\nVender: " +
                    shopItem.sellPrice +
                    " monedas";
            }

            Button button =
                buttonObject.GetComponent<Button>();

            if (button != null)
            {
                GameObject itemToSell =
                    slot.currenItem;

                button.onClick.AddListener(
                    () => SellItem(itemToSell)
                );
            }
        }
    }

    // =========================================================
    // VENDER
    // =========================================================

    private void SellItem(GameObject itemObject)
    {
        if (currentMerchant == null)
            return;

        if (itemObject == null)
        {
            CreateSellItems();
            return;
        }

        bool success =
            currentMerchant.SellItem(itemObject);

        if (success)
        {
            UpdateCoinsText();
        }

        CreateSellItems();
    }

    // =========================================================
    // MONEDAS
    // =========================================================

    private void UpdateCoinsText()
    {
        if (coinsText == null)
            return;

        if (GameManager.Instance == null)
            return;

        coinsText.text =
            "Monedas: " +
            GameManager.Instance.coins;
    }

    // =========================================================
    // LIMPIAR CONTENEDOR
    // =========================================================

    private void ClearContainer(Transform container)
    {
        if (container == null)
            return;

        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
}