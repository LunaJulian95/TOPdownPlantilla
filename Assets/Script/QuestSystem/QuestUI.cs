
using TMPro;
using UnityEngine;
using static Quest;

public class QuestUI : MonoBehaviour
{
    public Transform questListContent;
    public GameObject questEntryPrefab;
    public GameObject objetivesTextPrefab;

    void Start()
    {
        UpdateQuestUI();
    }

    public void UpdateQuestUI()
    {
        if (questListContent == null ||
            questEntryPrefab == null ||
            objetivesTextPrefab == null)
        {
            Debug.LogError(
                "QuestUI: faltan referencias en el Inspector."
            );
            return;
        }

        if (QuestController.Instance == null)
        {
            Debug.LogError(
                "QuestUI: no existe QuestController en la escena."
            );
            return;
        }

        // Eliminar las entradas anteriores.
        foreach (Transform child in questListContent)
        {
            Destroy(child.gameObject);
        }

        // Mostrar las misiones activas.
        foreach (QuestProgress questProgress
                 in QuestController.Instance.activeQuest)
        {
            if (questProgress == null)
                continue;

            // La referencia al ScriptableObject puede perderse
            // al cargar el progreso guardado.
            if (questProgress.quest == null)
            {
                Debug.LogError(
                    "QuestUI: una misión cargada no tiene asignado " +
                    "su objeto Quest. Hay que restaurar la referencia " +
                    "al cargar la partida."
                );
                continue;
            }

            GameObject entry = Instantiate(
                questEntryPrefab,
                questListContent
            );

            Transform nameTransform =
                entry.transform.Find("QuestNameTxt");

            Transform objectivesTransform =
                entry.transform.Find("ListaObjetivos");

            if (nameTransform == null ||
                objectivesTransform == null)
            {
                Debug.LogError(
                    "QuestUI: revisá que el prefab tenga los objetos " +
                    "'QuestNameTxt' y 'ListaObjetivos'."
                );

                Destroy(entry);
                continue;
            }

            TMP_Text questNameText =
                nameTransform.GetComponent<TMP_Text>();

            if (questNameText == null)
            {
                Debug.LogError(
                    "QuestUI: QuestNameTxt no tiene un componente TMP_Text."
                );

                Destroy(entry);
                continue;
            }

            // Usar el nombre configurado en el ScriptableObject.
            questNameText.text = questProgress.quest.questName;

            if (questProgress.objetives == null)
                continue;

            foreach (var objective in questProgress.objetives)
            {
                if (objective == null)
                    continue;

                GameObject objectiveTextObject = Instantiate(
                    objetivesTextPrefab,
                    objectivesTransform
                );

                TMP_Text objectiveText =
                    objectiveTextObject.GetComponent<TMP_Text>();

                if (objectiveText == null)
                {
                    Debug.LogError(
                        "QuestUI: el prefab de objetivos no tiene TMP_Text."
                    );

                    Destroy(objectiveTextObject);
                    continue;
                }

                objectiveText.text =
                    $"{objective.description} " +
                    $"({objective.currentAmount}/" +
                    $"{objective.requiredAmount})";
            }
        }
    }
}
