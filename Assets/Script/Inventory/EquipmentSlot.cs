using UnityEngine;
using UnityEngine.EventSystems;
using static Item;

public class EquipmentSlot : MonoBehaviour, IDropHandler
{
    public ItemType allowedType;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Drop en " + name);

        GameObject dragged = eventData.pointerDrag;
        if (dragged == null) return;

        Item item = dragged.GetComponent<Item>();
        if (item == null) return;

        if (item.itemType != allowedType)
        {
            Debug.Log("No es tipo " + allowedType);
            return;
        }

        dragged.transform.SetParent(transform);
        dragged.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        EquipmentManager.Instance.Equip(item);
        Debug.Log("Equipaste " + item.name + " daño: " + item.damageBonus);
    }
}