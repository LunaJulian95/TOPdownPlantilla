
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Item : MonoBehaviour, IPointerClickHandler
{
    public int ID;
    public string itemName;

    public enum ItemType
    {
        Weapon,
        Armor,
        Shield,
        Consumable,
        Misc
    }

    public enum AttackType
    {
        Melee,
        Projectile,
        Area,
        Aura
    }

    [Header("Equipable")]
    public ItemType itemType;
    public int damageBonus = 0;
    public int healthBonus = 0;
    public int blockBonus = 30;
    public int damageBonusSecondary = 5;

    [Header("Consumable / Drop")]
    public int healAmount = 25;
    public GameObject worldPrefab;

    [Header("Stack item")]
    public bool isStackable = false;
    public int maxStackSize = 99;
    public int quantity = 1;

    private TMP_Text quantityText;

    [Header("Proyectil")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 8f;

    [Header("Tipo de ataque")]
    public AttackType attackType = AttackType.Melee;

    [Header("Ataque en área")]
    public float areaRadius = 1.5f;
    public GameObject areaEffectPrefab;

    [HideInInspector]
    public Transform originalParent;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (transform.parent != transform.root)
            originalParent = transform.parent;

        quantityText = GetComponentInChildren<TMP_Text>();

        UpdateQuantityDisplay();
    }

    // =====================================================
    // STACK
    // =====================================================

    public void UpdateQuantityDisplay()
    {
        if (quantityText != null)
        {
            quantityText.text = quantity > 1
                ? quantity.ToString()
                : "";
        }
    }

    public void AddToStack(int amount = 1)
    {
        quantity += amount;
        UpdateQuantityDisplay();
    }

    public int RemoveFromStack(int amount = 1)
    {
        int removed = Mathf.Min(quantity, amount);

        quantity -= removed;

        UpdateQuantityDisplay();

        return removed;
    }

    public GameObject CloneItem(int newQuantity)
    {
        GameObject clone = Instantiate(gameObject);

        Item cloneItem = clone.GetComponent<Item>();

        if (cloneItem != null)
        {
            cloneItem.quantity = newQuantity;
            cloneItem.UpdateQuantityDisplay();
            cloneItem.originalParent = null;
        }

        return clone;
    }

    // =====================================================
    // RECOGER
    // =====================================================

    public virtual void ShowPopUp()
    {
        Image image = GetComponent<Image>();

        if (image == null)
            return;

        if (ItemPickUpUIController.Instance != null)
        {
            ItemPickUpUIController.Instance.ShowItemPickUp(
                itemName,
                image.sprite
            );
        }
    }

    // =====================================================
    // USAR OBJETO
    // =====================================================

    public void Use()
    {
        if (itemType == ItemType.Consumable)
        {
            Health playerHealth = null;

            foreach (Health health in FindObjectsOfType<Health>())
            {
                if (health.isPlayer)
                {
                    playerHealth = health;
                    break;
                }
            }

            if (playerHealth != null)
            {
                playerHealth.currentHealth += healAmount;

                if (playerHealth.currentHealth > playerHealth.maxHealth)
                    playerHealth.currentHealth = playerHealth.maxHealth;
            }

            RemoveFromStack(1);

            if (quantity > 0)
                return;

            if (originalParent != null)
            {
                Slot slot = originalParent.GetComponent<Slot>();

                if (slot != null)
                    slot.currenItem = null;
            }

            Destroy(gameObject);
        }
        else
        {
            EquipToSlot();
        }
    }

    // =====================================================
    // SOLTAR OBJETO
    // =====================================================

    public void Drop()
    {
        if (EquipmentManager.Instance != null)
            EquipmentManager.Instance.Unequip(this);

        if (originalParent != null)
        {
            Slot slot = originalParent.GetComponent<Slot>();

            if (slot != null)
                slot.currenItem = null;
        }

        Transform player = FindAnyObjectByType<PlayerMovement>()?.transform;

        if (player == null)
            player = FindAnyObjectByType<Health>()?.transform;

        if (player != null && worldPrefab != null)
        {
            Vector3 dropPos =
                player.position +
                player.forward * 1.5f +
                Vector3.up * 0.5f;

            Instantiate(worldPrefab, dropPos, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    // =====================================================
    // EQUIPAR
    // =====================================================

    public void EquipToSlot()
    {
        if (itemType == ItemType.Consumable)
        {
            Use();
            return;
        }

        if (EquipmentManager.Instance != null)
            EquipmentManager.Instance.Equip(this);
    }

    // =====================================================
    // CLICS DEL INVENTARIO
    // =====================================================

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            EquipToSlot();
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (itemType == ItemType.Consumable)
                SplitStack();

            return;
        }
    }

    // =====================================================
    // SEPARAR STACK
    // =====================================================

    private void SplitStack()
    {
        if (quantity <= 1)
            return;

        if (InventoryController.Instance == null)
            return;

        int splitAmount = quantity / 2;

        Slot freeSlot = null;

        foreach (Transform slotTransform in InventoryController.Instance.inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.currenItem == null)
            {
                freeSlot = slot;
                break;
            }
        }

        if (freeSlot == null)
        {
            Debug.Log("No hay espacio para separar el stack.");
            return;
        }

        RemoveFromStack(splitAmount);

        GameObject newItem = CloneItem(splitAmount);

        if (newItem == null)
        {
            AddToStack(splitAmount);
            return;
        }

        newItem.transform.SetParent(freeSlot.transform, false);

        RectTransform rect = newItem.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchoredPosition = Vector2.zero;
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
        }

        Item newItemComponent = newItem.GetComponent<Item>();

        if (newItemComponent != null)
            newItemComponent.originalParent = freeSlot.transform;

        freeSlot.currenItem = newItem;
    }
}
