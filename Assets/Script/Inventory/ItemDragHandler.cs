using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(BounceEffect))]
public class ItemDragHandler :
    MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IPointerClickHandler
{
    private Transform originalParent;
    private Slot originalSlot;

    private CanvasGroup canvasGroup;
    private Canvas menuCanvas;

    public float minDropDistance = 2f;
    public float maxDropDistance = 3f;

    private bool randomPlaceDrop = false;
    public float radiusOverlap = 0.5f;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        menuCanvas = GetComponentInParent<Canvas>();

        if (menuCanvas == null)
            menuCanvas = FindAnyObjectByType<Canvas>();
    }


    // =========================================================
    // BEGIN DRAG
    // =========================================================

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Guardamos el padre ORIGINAL
        originalParent = transform.parent;

        // Guardamos el SLOT ORIGINAL
        originalSlot = originalParent.GetComponent<Slot>();

        if (menuCanvas == null)
            menuCanvas = GetComponentInParent<Canvas>();

        if (menuCanvas == null)
            menuCanvas = FindAnyObjectByType<Canvas>();

        if (menuCanvas == null)
        {
            Debug.LogError("ItemDragHandler: No se encontró ningún Canvas.");
            return;
        }

        // IMPORTANTE:
        // quitamos temporalmente el item del slot,
        // pero NO destruimos la referencia original.
        if (originalSlot != null)
        {
            originalSlot.currenItem = null;
        }

        transform.SetParent(menuCanvas.transform);

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }


    // =========================================================
    // DRAG
    // =========================================================

    public void OnDrag(PointerEventData eventData)
    {
        if (menuCanvas == null)
            return;

        transform.position = eventData.position;
    }


    // =========================================================
    // END DRAG
    // =========================================================

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // -----------------------------------------------------
        // BUSCAR SLOT DESTINO
        // -----------------------------------------------------

        Slot dropSlot = null;

        if (eventData.pointerEnter != null)
        {
            dropSlot =
                eventData.pointerEnter.GetComponentInParent<Slot>();
        }


        // -----------------------------------------------------
        // SI NO ENCONTRÓ SLOT
        // -----------------------------------------------------
        if (dropSlot == null)
        {
            DropItem(originalSlot);
            return;
        }


        // -----------------------------------------------------
        // SOLTÓ EN EL MISMO SLOT
        // -----------------------------------------------------

        if (dropSlot == originalSlot)
        {
            ReturnToOriginalSlot();
            return;
        }


        // -----------------------------------------------------
        // SLOT VACÍO
        // -----------------------------------------------------

        if (dropSlot.currenItem == null)
        {
            MoveToSlot(dropSlot);
            return;
        }


        // -----------------------------------------------------
        // SLOT OCUPADO
        // -----------------------------------------------------

        GameObject targetObject = dropSlot.currenItem;

        Item draggedItem = GetComponent<Item>();
        Item targetItem = targetObject.GetComponent<Item>();


        if (draggedItem == null || targetItem == null)
        {
            ReturnToOriginalSlot();
            return;
        }


        // -----------------------------------------------------
        // MISMO ITEM → APILAR
        // -----------------------------------------------------

        if (draggedItem.ID == targetItem.ID &&
     draggedItem.itemType == Item.ItemType.Consumable &&
     targetItem.itemType == Item.ItemType.Consumable)
        {
            Debug.Log("===== APILANDO ITEMS =====");

            int amountToAdd = draggedItem.quantity;

            if (amountToAdd < 1)
                amountToAdd = 1;

            // Sumamos la cantidad al item destino
            targetItem.AddToStack(amountToAdd);

            // El slot original queda vacío
            if (originalSlot != null)
                originalSlot.currenItem = null;

            // Eliminamos el objeto que estábamos arrastrando
            Destroy(gameObject);

            Debug.Log("Items apilados correctamente.");
            return;
        }


        // -----------------------------------------------------
        // ITEMS DIFERENTES → INTERCAMBIAR
        // -----------------------------------------------------

        GameObject draggedObject = gameObject;

        Transform originalSlotTransform = originalParent;

        // Item que estaba en destino → slot original
        targetObject.transform.SetParent(
            originalSlotTransform,
            false
        );

        RectTransform targetRect =
            targetObject.GetComponent<RectTransform>();

        if (targetRect != null)
        {
            targetRect.anchoredPosition = Vector2.zero;
            targetRect.localRotation = Quaternion.identity;
            targetRect.localScale = Vector3.one;
        }


        // Item arrastrado → slot destino
        draggedObject.transform.SetParent(
            dropSlot.transform,
            false
        );

        RectTransform draggedRect =
            draggedObject.GetComponent<RectTransform>();

        if (draggedRect != null)
        {
            draggedRect.anchoredPosition = Vector2.zero;
            draggedRect.localRotation = Quaternion.identity;
            draggedRect.localScale = Vector3.one;
        }


        // Actualizar referencias
        if (originalSlot != null)
            originalSlot.currenItem = targetObject;

        dropSlot.currenItem = draggedObject;


        // Actualizar originalParent del Item
        UpdateItemParent(targetObject, originalSlotTransform);
        UpdateItemParent(draggedObject, dropSlot.transform);
    }


    // =========================================================
    // VOLVER AL SLOT ORIGINAL
    // =========================================================

    private void ReturnToOriginalSlot()
    {
        if (originalSlot == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.SetParent(
            originalSlot.transform,
            false
        );

        RectTransform rect =
            GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchoredPosition = Vector2.zero;
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
        }

        originalSlot.currenItem = gameObject;

        UpdateItemParent(
            gameObject,
            originalSlot.transform
        );
    }


    // =========================================================
    // MOVER A SLOT VACÍO
    // =========================================================

    private void MoveToSlot(Slot destinationSlot)
    {
        if (originalSlot != null)
            originalSlot.currenItem = null;

        transform.SetParent(
            destinationSlot.transform,
            false
        );

        RectTransform rect =
            GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchoredPosition = Vector2.zero;
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
        }

        destinationSlot.currenItem = gameObject;

        UpdateItemParent(
            gameObject,
            destinationSlot.transform
        );
    }


    // =========================================================
    // ACTUALIZAR ORIGINAL PARENT DEL ITEM
    // =========================================================

    private void UpdateItemParent(
        GameObject itemObject,
        Transform newParent
    )
    {
        Item item = itemObject.GetComponent<Item>();

        if (item != null)
            item.originalParent = newParent;
    }


    // =========================================================
    // CLICK DERECHO → SPLIT
    // =========================================================

    public void OnPointerClick(
        PointerEventData eventData
    )
    {
        if (eventData.button ==
            PointerEventData.InputButton.Right)
        {
            Item item = GetComponent<Item>();

            if (item == null)
                return;

            SplitStack(item);
        }
    }


    // =========================================================
    // SPLIT STACK
    // =========================================================

    private void SplitStack(Item item)
    {
        if (item.quantity <= 1)
            return;

        if (InventoryController.Instance == null)
            return;

        // Buscar slot vacío
        Slot freeSlot = null;

        foreach (
            Transform slotTransform
            in InventoryController.Instance.inventoryPanel.transform
        )
        {
            Slot slot =
                slotTransform.GetComponent<Slot>();

            if (slot != null &&
                slot.currenItem == null)
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

        // Dividir
        int splitAmount = item.quantity / 2;

        item.RemoveFromStack(splitAmount);

        // Crear clon
        GameObject newItem =
            item.CloneItem(splitAmount);

        if (newItem == null)
        {
            item.AddToStack(splitAmount);
            return;
        }

        // Colocarlo en el slot
        newItem.transform.SetParent(
            freeSlot.transform,
            false
        );

        RectTransform rect =
            newItem.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchoredPosition = Vector2.zero;
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
        }

        // Actualizar Slot
        freeSlot.currenItem = newItem;

        // Actualizar Item
        Item newItemComponent =
            newItem.GetComponent<Item>();

        if (newItemComponent != null)
        {
            newItemComponent.originalParent =
                freeSlot.transform;
        }
    }


    // =========================================================
    // DROP FUERA DEL INVENTARIO
    // =========================================================

    private void DropItem(Slot slot)
    {
        Item item = GetComponent<Item>();

        if (item == null)
            return;

        // Buscar jugador
        Transform player =
            GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("No se encontró el Player.");
            ReturnToOriginalSlot();
            return;
        }

        // Verificar que exista prefab del mundo
        if (item.worldPrefab == null)
        {
            Debug.LogError(
                "El item '" + item.itemName +
                "' no tiene asignado World Prefab."
            );

            ReturnToOriginalSlot();
            return;
        }

        // Posición donde aparecerá el objeto
        Vector3 dropPosition =
            player.position + Vector3.right;

        // =========================================
        // SI HAY MÁS DE UNO
        // =========================================

        if (item.quantity > 1)
        {
            // Sacar UNA unidad del stack
            item.RemoveFromStack(1);

            // Crear UNA unidad en el mundo
            GameObject dropItem = Instantiate(
                item.worldPrefab,
                dropPosition,
                Quaternion.identity
            );

            // Si el prefab tiene Item, establecer cantidad
            Item droppedItem = dropItem.GetComponent<Item>();

            if (droppedItem != null)
            {
                droppedItem.quantity = 1;
                droppedItem.UpdateQuantityDisplay();
            }

            BounceEffect bounce =
                dropItem.GetComponent<BounceEffect>();

            if (bounce != null)
                bounce.StartBounce();

            // El item original sigue en su slot
            ReturnToOriginalSlot();

            return;
        }

        // =========================================
        // SI SOLO HAY UNO
        // =========================================

        if (slot != null)
            slot.currenItem = null;

        // Crear objeto en el mundo
        GameObject droppedObject = Instantiate(
            item.worldPrefab,
            dropPosition,
            Quaternion.identity
        );

        BounceEffect droppedBounce =
            droppedObject.GetComponent<BounceEffect>();

        if (droppedBounce != null)
            droppedBounce.StartBounce();

        // Eliminar el objeto de UI
        Destroy(gameObject);
    }

    // =========================================================
    // INVENTARIO
    // =========================================================

    private bool IsWithinInventory(
        Vector2 mousePosition
    )
    {
        if (originalParent == null)
            return false;

        if (originalParent.parent == null)
            return false;

        RectTransform inventoryRect =
            originalParent.parent
            .GetComponent<RectTransform>();

        if (inventoryRect == null)
            return false;

        return RectTransformUtility
            .RectangleContainsScreenPoint(
                inventoryRect,
                mousePosition
            );
    }
}