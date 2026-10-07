using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEditor.SceneManagement;

public class Item : MonoBehaviour, IPointerClickHandler
{
    public int ID;
    public string itemName;
    public enum ItemType { Weapon, Armor, Shield, Consumable, Misc }

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
    public int quantity ;
    private TMP_Text quantityText;

    [HideInInspector] public Transform originalParent;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        if (transform.parent != transform.root) originalParent = transform.parent;
        quantityText = GetComponentInChildren<TMP_Text>();
        UpdateQuantityDisplay();
    }

    public void UpdateQuantityDisplay()
    {
        if(quantityText != null)
        {
            quantityText.text =(quantity > 1) ? quantity.ToString() : "";
        }
    }

    public void AddToStack(int amount = 1)
    {
        this.quantity += amount;
        
        UpdateQuantityDisplay();
    }

    public int RemoveFromStack(int amount = 1)
    {
        int removed = Mathf.Min(this.quantity, amount);
        this.quantity -= removed;
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

    public virtual void PickUp()
    {
        Sprite sprite = GetComponent<Image>().sprite;
        if (ItemPickUpUIController.Instance != null)
            ItemPickUpUIController.Instance.ShowItemPickUp(itemName, sprite);
    }

    public void Use()
    {
        if (itemType == ItemType.Consumable)
        {
            // =========================================
            // USAR UNA SOLA POCIÓN
            // =========================================

            Health h = null;

            foreach (var x in FindObjectsOfType<Health>())
            {
                if (x.isPlayer)
                {
                    h = x;
                    break;
                }
            }

            if (h != null)
            {
                h.currentHealth += healAmount;

                if (h.currentHealth > h.maxHealth)
                {
                    h.currentHealth = h.maxHealth;
                }
            }


            // =========================================
            // QUITAR 1 UNIDAD DE LA PILA
            // =========================================

            RemoveFromStack(1);


            // =========================================
            // SI TODAVÍA QUEDAN POCIONES
            // =========================================

            if (quantity > 0)
            {
                return;
            }


            // =========================================
            // SE TERMINÓ LA PILA
            // =========================================

            if (originalParent != null)
            {
                Slot s = originalParent.GetComponent<Slot>();

                if (s != null)
                {
                    s.currenItem = null;
                }
            }

            Destroy(gameObject);
        }
        else
        {
            EquipToSlot();
        }
    }

    public void Drop()
    {
        if (EquipmentManager.Instance != null) EquipmentManager.Instance.Unequip(this);
        if (originalParent != null)
        {
            Slot s = originalParent.GetComponent<Slot>();
            if (s != null) s.currenItem = null;
        }
        Transform player = FindAnyObjectByType<PlayerMovement>()?.transform;
        if (player == null) player = FindAnyObjectByType<Health>()?.transform;
        if (player != null && worldPrefab != null)
        {
            Vector3 dropPos = player.position + player.forward * 1.5f + Vector3.up * 0.5f;
            Instantiate(worldPrefab, dropPos, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    public void EquipToSlot()
    {
        if (itemType == ItemType.Consumable) { Use(); return; }
        if (EquipmentManager.Instance != null) EquipmentManager.Instance.Equip(this);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        // CLICK IZQUIERDO
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            EquipToSlot();
            return;
        }

        // CLICK DERECHO
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Solo los consumibles pueden separarse
            if (itemType == ItemType.Consumable)
            {
                SplitStack();
            }

            return;
        }
    }


    private void SplitStack()
    {
        if (quantity <= 1)
            return;

        if (InventoryController.Instance == null)
            return;

        int splitAmount = quantity / 2;

        // Buscar primero un slot vacío
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

        // Si no hay espacio, no hacemos nada
        if (freeSlot == null)
        {
            Debug.Log("No hay espacio para separar el stack.");
            return;
        }

        // Quitamos la mitad del stack original
        RemoveFromStack(splitAmount);

        // Creamos el nuevo objeto
        GameObject newItem = CloneItem(splitAmount);

        if (newItem == null)
        {
            AddToStack(splitAmount);
            return;
        }

        // Colocamos el nuevo objeto en el slot
        newItem.transform.SetParent(freeSlot.transform, false);

        RectTransform rect = newItem.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchoredPosition = Vector2.zero;
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
        }

        // MUY IMPORTANTE:
        // El nuevo item debe saber cuál es su slot
        Item newItemComponent = newItem.GetComponent<Item>();

        if (newItemComponent != null)
        {
            newItemComponent.originalParent = freeSlot.transform;
        }

        // El slot también debe saber qué item contiene
        freeSlot.currenItem = newItem;

        Debug.Log(
            "Stack separado: " +
            quantity +
            " + " +
            splitAmount
        );
    }

}
