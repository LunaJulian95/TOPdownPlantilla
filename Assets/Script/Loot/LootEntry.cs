using UnityEngine;

[System.Serializable]
public class LootEntry
{
    public GameObject itemPrefab;

    [Range(0f, 100f)]
    public float dropChance = 50f;

    public int minAmount = 1;
    public int maxAmount = 1;
}