using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public GameObject currenItem;

    public void OnDrop(PointerEventData eventData)
    {
        Item draggedItem = eventData.pointerDrag?.GetComponent<Item>();
        if (draggedItem == null) return;

        // Si este slot ya está ocupado, no dejar soltar
        if (currenItem != null && currenItem != draggedItem.gameObject) return;

        // Limpiar el slot viejo
        if (draggedItem.originalParent != null)
        {
            Slot oldSlot = draggedItem.originalParent.GetComponent<Slot>();
            if (oldSlot != null) oldSlot.currenItem = null;
        }

        // Mover al nuevo slot
        draggedItem.originalParent = transform;
        draggedItem.transform.SetParent(transform);
        draggedItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        currenItem = draggedItem.gameObject;
    }

    void Update()
    {
        // FIX DEL CLON: si el objeto fue destruido, limpia la referencia
        if (currenItem != null && currenItem.Equals(null))
        {
            currenItem = null;
        }
        if (transform.childCount == 0)
        {
            currenItem = null;
        }
        else if (currenItem == null && transform.childCount > 0)
        {
            // si hay un hijo pero no está registrado, lo registra
            currenItem = transform.GetChild(0).gameObject;
        }
    }
}