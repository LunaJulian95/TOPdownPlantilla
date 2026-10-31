
using UnityEngine;

public class RewardsController : MonoBehaviour
{
    public static RewardsController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GiveQuestReward(Quest quest)
    {
        if (quest == null || quest.questRewards == null)
            return;

        foreach (var reward in quest.questRewards)
        {
            switch (reward.type)
            {
                case RewardType.Item:

                    GiveItemReward(reward.rewardID, reward.amount);

                    break;

                case RewardType.Experience:

                    // Pendiente de conectar con PlayerLevel.

                    break;

                case RewardType.Custom:

                    // Pendiente de implementar.

                    break;

                case RewardType.Gold:

                    GiveGoldReward(reward.amount);

                    break;
            }
        }
    }

    // =====================================================
    // RECOMPENSA DE OBJETOS
    // =====================================================

    public void GiveItemReward(int itemID, int amount)
    {
        ItemDiccionary dictionary =
            FindAnyObjectByType<ItemDiccionary>();

        if (dictionary == null)
        {
            Debug.LogError("No se encontró ItemDiccionary.");
            return;
        }

        GameObject itemPrefab = dictionary.GetItemPrefab(itemID);

        if (itemPrefab == null)
        {
            Debug.LogError("No existe un prefab para el ID: " + itemID);
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            if (InventoryController.Instance == null)
            {
                Debug.LogError("No existe InventoryController.");
                return;
            }

            if (!InventoryController.Instance.AddItem(itemPrefab))
            {
                GameObject dropItem = Instantiate(
                    itemPrefab,
                    transform.position + Vector3.down,
                    Quaternion.identity
                );

                BounceEffect bounce = dropItem.GetComponent<BounceEffect>();

                if (bounce != null)
                    bounce.StartBounce();
            }
            else
            {
                Item item = itemPrefab.GetComponent<Item>();

                if (item != null)
                    item.ShowPopUp();
            }
        }
    }

    // =====================================================
    // RECOMPENSA DE MONEDAS
    // =====================================================

    public void GiveGoldReward(int amount)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("No existe GameManager en la escena.");
            return;
        }

        GameManager.Instance.AddCoins(amount);

        Debug.Log("Recompensa recibida: " + amount + " monedas.");
    }
}
