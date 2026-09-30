using UnityEngine;

[System.Serializable]
public class ShopItem
{
    public GameObject itemPrefab;

    [Header("Precios")]
    public int buyPrice = 50;
    public int sellPrice = 25;
}