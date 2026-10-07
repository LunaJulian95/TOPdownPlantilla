using UnityEngine;

public class WorldLoot : MonoBehaviour
{
    private bool pickedUp = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp)
            return;

        if (!other.CompareTag("Player"))
            return;

        Item worldItem = GetComponent<Item>();

        if (worldItem == null)
        {
            Debug.LogError(
                "WorldLoot: " +
                gameObject.name +
                " no tiene componente Item."
            );
            return;
        }

        if (InventoryController.Instance == null)
        {
            Debug.LogError("No existe InventoryController.");
            return;
        }

        pickedUp = true;

        int amount = worldItem.quantity;

        if (amount < 1)
            amount = 1;

        Debug.Log(
            "========== PICKUP ==========\n" +
            "Objeto: " + gameObject.name + "\n" +
            "ID: " + worldItem.ID + "\n" +
            "Cantidad: " + amount
        );

        bool added =
            InventoryController.Instance.AddWorldItemByID(
                worldItem.ID,
                amount
            );

        Debug.Log(
            "Resultado AddWorldItemByID: " + added
        );

        if (added)
        {
            Debug.Log("DESTRUYENDO OBJETO DEL SUELO: " + gameObject.name);

            Destroy(gameObject);
        }
        else
        {
            Debug.Log("NO SE PUDO RECOGER EL ITEM.");

            pickedUp = false;
        }
    }
}