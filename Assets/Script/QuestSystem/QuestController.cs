
using System.Collections.Generic;
using UnityEngine;
using static Quest;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance { get; private set; }

    public List<QuestProgress> activeQuest = new List<QuestProgress>();

    private QuestUI questUI;
    private InventoryController inventory;
    public List<Quest> allQuests = new List<Quest>();

    public List<string> handinQuestIDs =new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        questUI = FindObjectOfType<QuestUI>();
        inventory = FindObjectOfType<InventoryController>();
    }

    private void OnEnable()
    {
        // La suscripción se realiza en Start, cuando las instancias
        // de la escena ya tuvieron oportunidad de inicializarse.
    }

    private void Start()
    {
        if (inventory == null)
            inventory = InventoryController.Instance;

        if (questUI == null)
            questUI = FindObjectOfType<QuestUI>();

        if (inventory == null)
        {
            Debug.LogError(
                "QuestController: no se encontró InventoryController."
            );
            return;
        }

        inventory.OnInventoryChanged += CheckInvetoryForQuests;

        // Revisar el inventario inicial.
        CheckInvetoryForQuests();
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= CheckInvetoryForQuests;

        if (Instance == this)
            Instance = null;
    }

    public void AcceptQuest(Quest quest)
    {
        if (quest == null)
        {
            Debug.LogError("No se puede aceptar una quest nula.");
            return;
        }

        if (IsQuestActive(quest.questID))
            return;

        activeQuest.Add(new QuestProgress(quest));

        CheckInvetoryForQuests();

        Debug.Log("Quest aceptada: " + quest.questName);

        // Comprobar si ya hay objetos en el inventario.
        CheckInvetoryForQuests();
    }

    public bool IsQuestActive(string questID)
    {
        return activeQuest.Exists(q => q.QuestID == questID);
    }

    public void CheckInvetoryForQuests()
    {
        // Comprobar que exista el inventario.
        if (InventoryController.Instance == null)
        {
            Debug.LogWarning(
                "QuestController: InventoryController.Instance es null."
            );
            return;
        }

        // Comprobar que el inventario tenga sus paneles.
        if (InventoryController.Instance.inventoryPanel == null)
        {
            Debug.LogWarning(
                "QuestController: inventoryPanel no está asignado."
            );
            return;
        }

        // Comprobar que exista la lista de quests.
        if (activeQuest == null)
            activeQuest = new List<QuestProgress>();

        Dictionary<int, int> itemCounts =
            InventoryController.Instance.GetItemCount();

        if (itemCounts == null)
        {
            Debug.LogWarning(
                "QuestController: GetItemCount devolvió null."
            );
            return;
        }

        foreach (QuestProgress quest in activeQuest)
        {
            if (quest == null || quest.objetives == null)
                continue;

            foreach (QuestObjetive objective in quest.objetives)
            {
                if (objective == null)
                    continue;

                if (objective.type != ObjectiveType.Collection)
                    continue;

                if (!int.TryParse(objective.objetiveID, out int itemID))
                {
                    Debug.LogWarning(
                        "ID de objetivo inválido: " + objective.objetiveID
                    );
                    continue;
                }

                int amount = itemCounts.TryGetValue(itemID, out int count)
                    ? Mathf.Min(count, objective.requiredAmount)
                    : 0;

                objective.currentAmount = amount;
            }
        }

        // Actualizar la interfaz solo si existe.
        if (questUI != null)
            questUI.UpdateQuestUI();
        else
            Debug.LogWarning(
                "QuestController: no se encontró QuestUI."
            );
    }


    public bool IsQuestCompleted(string questID)
    {
        QuestProgress quest = activeQuest.Find(q =>q.QuestID == questID);
        return quest != null && quest.objetives.TrueForAll(o => o.IsCompleted);
    }

    public void HandInQuest (string questID)
    {
        //Try remove required item 
        if (!RemoveRequiredItemsFromInventory(questID))
        {
            return;
        }


        //Remove quest from quest log
        QuestProgress quest = activeQuest.Find(q => q.QuestID == questID);
        if(quest != null)
        {
            handinQuestIDs.Add(questID);
            activeQuest.Remove(quest);
            questUI.UpdateQuestUI();
        }
    }


    public bool IsQuestHandedIn(string questID)
    {
        return handinQuestIDs.Contains(questID);
    }

    public bool RemoveRequiredItemsFromInventory(string questID)
    {
        QuestProgress quest = activeQuest.Find(q => q.QuestID == questID);
        if (quest == null) return false;

        Dictionary<int, int> requiredItems = new();

        foreach(QuestObjetive objetive in quest.objetives)
        {
            if(objetive.type == ObjectiveType.Collection && int.TryParse(objetive.objetiveID, out int itemID))
            {
                requiredItems[itemID] = objetive.requiredAmount;
            }
        }

        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCount();
        foreach(var item in requiredItems)
        {
            if (itemCounts.GetValueOrDefault(item.Key) < item.Value)
            {
                return false;
            }
        }

        foreach (var itemRequired in requiredItems)
        {
            InventoryController.Instance.RemoveItemFromInventory(itemRequired.Key, itemRequired.Value);
        }

        return true;
    }

    public void LoadQuestProgress(List<QuestProgress> saveQuests)
    {
        if (saveQuests == null)
        {
            Debug.LogWarning("No hay progreso de quests para cargar.");
            return;
        }

        activeQuest = saveQuests;

        CheckInvetoryForQuests();

        if (questUI != null)
            questUI.UpdateQuestUI();
    }


    public List<QuestProgressSaveData> GetQuestProgressForSave()
    {
        List<QuestProgressSaveData> result =
            new List<QuestProgressSaveData>();

        foreach (QuestProgress progress in activeQuest)
        {
            if (progress == null || progress.quest == null)
                continue;

            QuestProgressSaveData savedQuest =
                new QuestProgressSaveData();

            savedQuest.questID = progress.quest.questID;

            if (progress.objetives != null)
            {
                foreach (QuestObjetive objective in progress.objetives)
                {
                    if (objective == null)
                        continue;

                    savedQuest.objectives.Add(
                        new QuestObjectiveSaveData
                        {
                            objectiveID = objective.objetiveID,
                            currentAmount = objective.currentAmount
                        }
                    );
                }
            }

            result.Add(savedQuest);
        }

        return result;
    }

    public void LoadQuestProgress(
        List<QuestProgressSaveData> savedQuests)
    {
        activeQuest.Clear();

        if (savedQuests == null)
        {
            Debug.Log("No hay misiones guardadas.");
            if (questUI != null)
                questUI.UpdateQuestUI();
            return;
        }

        foreach (QuestProgressSaveData savedQuest in savedQuests)
        {
            if (savedQuest == null ||
                string.IsNullOrEmpty(savedQuest.questID))
                continue;

            Quest originalQuest = allQuests.Find(
                q => q != null && q.questID == savedQuest.questID
            );

            if (originalQuest == null)
            {
                Debug.LogWarning(
                    "No se encontró el asset de la misión con ID: " +
                    savedQuest.questID
                );
                continue;
            }

            QuestProgress progress = new QuestProgress(originalQuest);

            if (savedQuest.objectives != null &&
                progress.objetives != null)
            {
                foreach (QuestObjectiveSaveData savedObjective
                         in savedQuest.objectives)
                {
                    if (savedObjective == null)
                        continue;

                    QuestObjetive objective = progress.objetives.Find(
                        o => o.objetiveID == savedObjective.objectiveID
                    );

                    if (objective != null)
                    {
                        objective.currentAmount = Mathf.Clamp(
                            savedObjective.currentAmount,
                            0,
                            objective.requiredAmount
                        );
                    }
                }
            }

            activeQuest.Add(progress);
        }

        // No recalcular aquí las misiones todavía:
        // el inventario podría no haberse cargado por completo.
        if (questUI != null)
            questUI.UpdateQuestUI();
    }

}
