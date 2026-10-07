using System.Collections.Generic;
using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryController inventoryController;

    // Items que ya están siendo recogidos
    private HashSet<Item> itemsBeingCollected = new HashSet<Item>();

    private void Start()
    {
        inventoryController = FindAnyObjectByType<InventoryController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("item"))
            return;

        if (inventoryController == null)
        {
            Debug.LogError("No se encontró InventoryController.");
            return;
        }

        // Buscar Item en el objeto que entró al trigger
        Item item = collision.GetComponent<Item>();

        // Si el collider está en un hijo del prefab
        if (item == null)
        {
            item = collision.GetComponentInParent<Item>();
        }

        if (item == null)
            return;

        // =====================================================
        // EVITAR RECOGER EL MISMO ITEM MÁS DE UNA VEZ
        // =====================================================

        if (itemsBeingCollected.Contains(item))
            return;

        itemsBeingCollected.Add(item);

        // =====================================================
        // CANTIDAD
        // =====================================================

        int amount = item.quantity;

        if (amount < 1)
            amount = 1;

        Debug.Log(
            "Recogiendo: " +
            item.itemName +
            " x" +
            amount
        );

        // =====================================================
        // DESACTIVAR TODOS LOS COLLIDERS DEL ITEM
        // =====================================================

        Collider2D[] itemColliders =
            item.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D itemCollider in itemColliders)
        {
            itemCollider.enabled = false;
        }

        // =====================================================
        // AGREGAR AL INVENTARIO
        // =====================================================

        bool added =
            inventoryController.AddWorldItemByID(
                item.ID,
                amount
            );

        // =====================================================
        // SI SE AGREGÓ CORRECTAMENTE
        // =====================================================

        if (added)
        {
            item.PickUp();

            Destroy(item.gameObject);
        }
        else
        {
            // Si el inventario está lleno,
            // permitir recogerlo nuevamente.

            foreach (Collider2D itemCollider in itemColliders)
            {
                if (itemCollider != null)
                    itemCollider.enabled = true;
            }

            itemsBeingCollected.Remove(item);
        }
    }
}