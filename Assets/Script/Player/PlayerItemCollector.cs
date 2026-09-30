using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryController inventoryController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryController = FindAnyObjectByType<InventoryController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("item"))
        {
            Item item = collision.GetComponent<Item>();
            if (item != null)
            {
                bool added = inventoryController.AddItem(collision.gameObject);
                if(added)
                {
                    item.PickUp();
                    Destroy(collision.gameObject);
                }

            }
        }
    }

}
