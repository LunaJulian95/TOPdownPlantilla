using UnityEngine;

public class Chest : MonoBehaviour, Iinteractable
{

    private const string CHEST_SOUND_NAME = "Chest";
    public bool IsOpened { get; private set; } 

    public string ChestID { get; private set; } // Unique identifier for the chest

    public GameObject itemPrefab; // The item prefab to spawn when the chest is opened

    public Sprite openedSprite; // The sprite to display when the chest is opened

    public GameObject minimapBlip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChestID ??= GlobarHelper.GenerateUniqueID(gameObject); // Generate a unique ID for the chest if it doesn't already have one
    }

    

    public bool CanInteract()
    {
        return !IsOpened; // The chest can be interacted with if it is not already opened
    }

    public void Interact()
    {
        if(!CanInteract())
        {
            return; // If the chest cannot be interacted with, exit the method

        }
        OpenChest(); // Open the chest
    }

    private void OpenChest()
    {
        SetOpened(true);
        SoundEffectManager.Instance.Play(CHEST_SOUND_NAME);





        //soltar items
        if(itemPrefab != null)
        {
            GameObject item = Instantiate(itemPrefab,
            transform.position + Vector3.down, Quaternion.identity); // Spawn the item at the chest's position
            item.GetComponent<BounceEffect>().StartBounce(); // Start the bounce effect on the spawned item
        }
    }

    public void SetOpened (bool opened) 
    {
        if (!IsOpened && opened) 
        {
            IsOpened = true; // Set the chest as opened
            GetComponent<SpriteRenderer>().sprite = openedSprite; // Change the sprite to the opened sprite
            //minimapBlip.GetComponent<SpriteRenderer>().sprite = openedSprite;
            minimapBlip.SetActive(false);
        }
    }
}
