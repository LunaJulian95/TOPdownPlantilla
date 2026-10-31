
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryController inventoryController;

    // Items que ya están siendo recogidos.
    private HashSet<Item> itemsBeingCollected = new HashSet<Item>();

    private void Start()
    {
        inventoryController = FindAnyObjectByType<InventoryController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("item"))
            return;

        Item item = collision.GetComponent<Item>();

        if (item == null)
            item = collision.GetComponentInParent<Item>();

        if (item == null)
            return;

        // Las monedas no pertenecen al inventario.
        // Coin.cs se encarga de recogerlas.
        if (item.ID == 10 || item.GetComponent<Coin>() != null)
            return;

        if (inventoryController == null)
        {
            Debug.LogError("No se encontró InventoryController.");
            return;
        }

        if (itemsBeingCollected.Contains(item))
            return;

        itemsBeingCollected.Add(item);

        int amount = Mathf.Max(1, item.quantity);

        Debug.Log("Recogiendo: " + item.itemName + " x" + amount);

        Collider2D[] itemColliders =
            item.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D itemCollider in itemColliders)
            itemCollider.enabled = false;

        bool added = inventoryController.AddWorldItemByID(
            item.ID,
            amount
        );

        if (added)
        {
            item.ShowPopUp();
            Destroy(item.gameObject);
        }
        else
        {
            foreach (Collider2D itemCollider in itemColliders)
            {
                if (itemCollider != null)
                    itemCollider.enabled = true;
            }

            itemsBeingCollected.Remove(item);
        }
    }
}
