using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    public GameObject currenItem;


    // ==================================================
    // CLICK
    // ==================================================

    public void OnPointerClick(PointerEventData eventData)
    {
        // Solo clic izquierdo
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        // No hay objeto
        if (currenItem == null)
            return;

        // Solo se puede desequipar desde equipamiento
        if (!IsEquipmentSlot())
            return;

        UnequipCurrentItem();
    }


    // ==================================================
    // DESEQUIPAR
    // ==================================================

    private void UnequipCurrentItem()
    {
        Item item = currenItem.GetComponent<Item>();

        if (item == null)
        {
            Debug.LogWarning(
                "Slot: el objeto equipado no tiene componente Item."
            );
            return;
        }

        if (InventoryController.Instance == null)
        {
            Debug.LogWarning(
                "Slot: no existe InventoryController."
            );
            return;
        }

        if (EquipmentManager.Instance == null)
        {
            Debug.LogWarning(
                "Slot: no existe EquipmentManager."
            );
            return;
        }


        // Buscar panel de inventario
        GameObject inventoryPanel =
            InventoryController.Instance.inventoryPanel;

        if (inventoryPanel == null)
        {
            Debug.LogWarning(
                "Slot: inventoryPanel no está asignado."
            );
            return;
        }


        // ==================================================
        // BUSCAR SLOT LIBRE
        // ==================================================

        Slot freeSlot = null;

        foreach (Transform child in inventoryPanel.transform)
        {
            Slot slot = child.GetComponent<Slot>();

            if (slot != null && slot.currenItem == null)
            {
                freeSlot = slot;
                break;
            }
        }


        // No hay espacio
        if (freeSlot == null)
        {
            Debug.Log(
                "No hay espacio en el inventario para desequipar."
            );

            return;
        }


        // ==================================================
        // QUITAR EQUIPAMIENTO
        // ==================================================

        EquipmentManager.Instance.Unequip(item);


        // Limpiar slot de equipamiento
        currenItem = null;


        // ==================================================
        // MOVER AL INVENTARIO
        // ==================================================

        item.originalParent = freeSlot.transform;

        item.transform.SetParent(
            freeSlot.transform,
            false
        );


        // ==================================================
        // POSICIÓN Y ESCALA
        // ==================================================

        RectTransform rect =
            item.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchoredPosition = Vector2.zero;
            rect.localPosition = Vector3.zero;
            rect.localScale = Vector3.one;
        }


        // ==================================================
        // REGISTRAR EN EL SLOT
        // ==================================================

        freeSlot.currenItem = item.gameObject;
    }


    // ==================================================
    // SABER SI ES SLOT DE EQUIPAMIENTO
    // ==================================================

    private bool IsEquipmentSlot()
    {
        if (InventoryController.Instance == null)
            return false;

        if (InventoryController.Instance.equipPanel == null)
            return false;

        return transform.IsChildOf(
            InventoryController.Instance.equipPanel.transform
        );
    }
}