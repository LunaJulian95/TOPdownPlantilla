using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    [HideInInspector] public Transform originalParent;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        if (transform.parent != transform.root) originalParent = transform.parent;
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
            Health h = null;
            foreach (var x in FindObjectsOfType<Health>()) if (x.isPlayer) h = x;
            if (h != null)
            {
                h.currentHealth += healAmount;
                if (h.currentHealth > h.maxHealth) h.currentHealth = h.maxHealth;
            }
            // limpia slot
            if (originalParent != null)
            {
                Slot s = originalParent.GetComponent<Slot>();
                if (s != null) s.currenItem = null;
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
        if (eventData.button == PointerEventData.InputButton.Right) Use();
    }
    public void OnRightClick() { Use(); }
}