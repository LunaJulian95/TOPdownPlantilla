using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent (typeof(BounceEffect))]
public class ItemDragHandler :
    MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private Canvas menuCanvas;
    public float minDropDistance = 2f, maxDropDistance = 3f;


    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        // Busca el Canvas en los padres del item.
        menuCanvas = GetComponentInParent<Canvas>();

        if (menuCanvas == null)
        {
            // Si no lo encuentra, busca cualquier Canvas activo.
            menuCanvas = FindAnyObjectByType<Canvas>();
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;

        // Intentar encontrar el Canvas nuevamente
        // por si el objeto se creó después.
        if (menuCanvas == null)
        {
            menuCanvas = GetComponentInParent<Canvas>();
        }

        if (menuCanvas == null)
        {
            menuCanvas = FindAnyObjectByType<Canvas>();
        }

        if (menuCanvas == null)
        {
            Debug.LogError(
                "ItemDragHandler: No se encontró ningún Canvas."
            );

            return;
        }

        transform.SetParent(menuCanvas.transform);

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (menuCanvas == null)
            return;

        transform.position = eventData.position;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // ============================================
        // VERIFICAR SLOT ORIGINAL
        // ============================================

        if (originalParent == null)
        {
            Debug.LogWarning(
                "ItemDragHandler: originalParent es NULL."
            );

            return;
        }

        Slot originalSlot =
            originalParent.GetComponent<Slot>();


        // ============================================
        // BUSCAR SLOT DE DESTINO
        // ============================================

        Slot dropSlot = null;

        if (eventData.pointerEnter != null)
        {
            dropSlot =
                eventData.pointerEnter.GetComponentInParent<Slot>();
        }


        // ============================================
        // SI SE SOLTÓ SOBRE UN SLOT
        // ============================================

        if (dropSlot != null)
        {
            // Si es el mismo slot
            if (dropSlot == originalSlot)
            {
                transform.SetParent(originalParent);

                RectTransform rect =
                    GetComponent<RectTransform>();

                if (rect != null)
                {
                    rect.anchoredPosition = Vector2.zero;
                    rect.localPosition = Vector3.zero;
                }

                originalSlot.currenItem = gameObject;

                return;
            }


            // ========================================
            // SI EL SLOT DESTINO TIENE OTRO ITEM
            // ========================================

            if (dropSlot.currenItem != null)
            {
                GameObject otherItem =
                    dropSlot.currenItem;

                otherItem.transform.SetParent(
                    originalParent
                );

                RectTransform otherRect =
                    otherItem.GetComponent<RectTransform>();

                if (otherRect != null)
                {
                    otherRect.anchoredPosition =
                        Vector2.zero;

                    otherRect.localPosition =
                        Vector3.zero;
                }

                if (originalSlot != null)
                {
                    originalSlot.currenItem =
                        otherItem;
                }
            }
            else
            {
                if (originalSlot != null)
                {
                    originalSlot.currenItem = null;
                }
            }


            // ========================================
            // MOVER NUESTRO ITEM
            // ========================================

            transform.SetParent(
                dropSlot.transform
            );

            dropSlot.currenItem =
                gameObject;

            RectTransform itemRect =
                GetComponent<RectTransform>();

            if (itemRect != null)
            {
                itemRect.anchoredPosition =
                    Vector2.zero;

                itemRect.localPosition =
                    Vector3.zero;
            }

            return;
        }


        // ============================================
        // NO HAY SLOT → VOLVER AL ORIGINAL
        // ============================================

        Debug.Log(
            "Item soltado fuera de un Slot. Volviendo al slot original."
        );

        if(!IsWithinInventory(eventData.position))
        {
            Debug.Log(
                "dropea afuera del hot bar"
            );
            DropItem(originalSlot);
        }
        else 
        {

        }
            transform.SetParent(
                originalParent
            );

        RectTransform finalRect =
            GetComponent<RectTransform>();

        if (finalRect != null)
        {
            finalRect.anchoredPosition =
                Vector2.zero;

            finalRect.localPosition =
                Vector3.zero;

            finalRect.localScale =
                Vector3.one;
        }

        if (originalSlot != null)
        {
            originalSlot.currenItem =
                gameObject;
        }
    }

    bool IsWithinInventory(Vector2 mousePosition) 
    {
        RectTransform inventoryRect = originalParent.parent.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(inventoryRect, mousePosition);
    }

    void DropItem(Slot originalSlot)
    {
        originalSlot.currenItem = null;

        Transform playerPosition = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerPosition == null)
        {
            Debug.LogError("no player found");
            return;
        }

        // Generando una position aleatoria
        Vector2 dropOffSet = Random.insideUnitCircle * Random.Range(minDropDistance, maxDropDistance);
        Vector2 dropPosition = (Vector2)playerPosition.position + dropOffSet;

        //instanciar el objeto
        GameObject dropItem = Instantiate(gameObject, dropPosition, Quaternion.identity);
        dropItem.GetComponent<BounceEffect>().StartBounce();

        //elimina el objeto de la UI
        Destroy(gameObject);
}       }