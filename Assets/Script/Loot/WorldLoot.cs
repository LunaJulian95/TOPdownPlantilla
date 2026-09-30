using UnityEngine;

public class WorldLoot : MonoBehaviour
{
    public int id = 3; // ID del Hacha

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryController.Instance.AddItemByID(id);
            Destroy(gameObject);
        }
    }
}