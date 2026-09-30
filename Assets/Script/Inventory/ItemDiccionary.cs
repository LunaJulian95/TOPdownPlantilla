using System.Collections.Generic;
using UnityEngine;

public class ItemDiccionary : MonoBehaviour
{
    public List<Item> itemPrefabs;

    private Dictionary<int, GameObject> itemsDictionary;

    void Awake()
    {
        itemsDictionary = new Dictionary<int, GameObject>();

        for (int i = 0; i < itemPrefabs.Count; i++)
        {
            if (itemPrefabs[i] == null)
            {
                Debug.LogWarning("Hay un Item vacío en la posición " + i);
                continue;
            }

            int id = i + 1;

            itemPrefabs[i].ID = id;

            itemsDictionary[id] = itemPrefabs[i].gameObject;

            Debug.Log(
                "Item registrado: " +
                itemPrefabs[i].name +
                " | ID: " + id
            );
        }
    }

    public GameObject GetItemPrefab(int itemID)
    {
        if (itemsDictionary.TryGetValue(itemID, out GameObject prefab))
        {
            return prefab;
        }

        Debug.LogWarning(
            "Item ID " + itemID + " no encontrado en el diccionario."
        );

        return null;
    }
}