using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HotBarController : MonoBehaviour
{
    public GameObject hotBarPanel; 
    public GameObject slotPrefab;
    public int slotCount = 10; // Number of slots in the hotbar

    private ItemDiccionary itemDictionary;
    private Key[] hotBarKeys;

    void Awake()
    {
        itemDictionary = FindAnyObjectByType<ItemDiccionary>();

        hotBarKeys = new Key[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            hotBarKeys[i] = (i<9) ? (Key)((int)  Key.Digit1 + i) : Key.Digit0; // For 10th slot, use Key.Digit0
        }
    }


    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < slotCount; i++)
        {
           if(Keyboard.current[hotBarKeys[i]].wasPressedThisFrame)
           {
               
                UseItemInSlot(i);
            }
        }
    }

    void UseItemInSlot(int index)
    {
      Slot slot = hotBarPanel.transform.GetChild(index).GetComponent<Slot>();
        if(slot.currenItem != null) 
        {
            Item item = slot.currenItem.GetComponent<Item>();
            item.Use();
        }
    }
    public List<InvetorySaveData> GetHotBarItems()
    {
        List<InvetorySaveData> hotbarData =
            new List<InvetorySaveData>();

        foreach (Transform slotTransform in hotBarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.currenItem != null)
            {
                Item item =
                    slot.currenItem.GetComponent<Item>();

                if (item != null)
                {
                    hotbarData.Add(
                        new InvetorySaveData
                        {
                            itemIDs = item.ID,
                            slotIndex =
                                slotTransform.GetSiblingIndex()
                        }
                    );
                }
            }
        }

        return hotbarData;
    }
    public void SetHotBarItems(List<InvetorySaveData> hotbarData)
    {
        int totalSlots = slotCount;

        foreach (InvetorySaveData saveData in hotbarData)
        {
            if (saveData.slotIndex < totalSlots)
            {
                Slot slot =
                    hotBarPanel.transform
                    .GetChild(saveData.slotIndex)
                    .GetComponent<Slot>();

                GameObject itemPrefab =
                    itemDictionary.GetItemPrefab(saveData.itemIDs);

                if (itemPrefab != null)
                {
                    GameObject item =
                        Instantiate(
                            itemPrefab,
                            slot.transform
                        );

                    RectTransform rect =
                        item.GetComponent<RectTransform>();

                    if (rect != null)
                    {
                        rect.anchoredPosition = Vector2.zero;
                        rect.localScale = Vector3.one;
                    }

                    item.name = itemPrefab.name;

                    slot.currenItem = item;
                }
            }
        }
    }

}
