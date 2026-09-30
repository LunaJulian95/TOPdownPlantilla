using UnityEngine;

public class LootDrop : MonoBehaviour
{
    public LootTable lootTable;

    public void DropLoot()
    {
        if (lootTable == null)
        {
            Debug.LogWarning("No hay Loot Table asignada.");
            return;
        }

        foreach (LootEntry entry in lootTable.lootEntries)
        {
            // Tirada de probabilidad
            float roll = Random.Range(0f, 100f);

            if (roll <= entry.dropChance)
            {
                DropItem(entry);
            }
        }
    }

    private void DropItem(LootEntry entry)
    {
        if (entry.itemPrefab == null)
        {
            Debug.LogWarning("Hay un LootEntry sin prefab asignado.");
            return;
        }

        int amount = Random.Range(
            entry.minAmount,
            entry.maxAmount + 1
        );

        for (int i = 0; i < amount; i++)
        {
            Vector2 randomPos =
                (Vector2)transform.position +
                Random.insideUnitCircle * 0.5f;

            Instantiate(
                entry.itemPrefab,
                randomPos,
                Quaternion.identity
            );
        }
    }
}