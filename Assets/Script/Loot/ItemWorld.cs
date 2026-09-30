using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    public Item inventoryItemPrefab; // arrastra el prefab de UI de inventario
    public string itemName = "Poción";

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Health>()?.isPlayer == true)
        {
            // Busca el inventario
            Transform invContent = GameObject.Find("InventoryContent")?.transform;
            if (invContent == null) invContent = GameObject.Find("Content")?.transform;

            if (invContent != null && inventoryItemPrefab != null)
            {
                Instantiate(inventoryItemPrefab, invContent);
                Debug.Log("Recogiste " + itemName);
            }
            Destroy(gameObject);
        }
    }
}