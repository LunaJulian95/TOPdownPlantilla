using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{

    public static DialogueController Instance { get; private set; }

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text nameText, dialogueText;
    [SerializeField] private Image portraitImage;

    [SerializeField] private Transform choicesContainer;
    [SerializeField] private GameObject choiceButtonPrefab;

    public NPC npc;


    private void Awake()
    {
        if(Instance == null)
        
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Muestra u oculta el panel del dialogo
    /// </summary>
    /// <param name="show">Muestra u oculta el Panel</param>



    public void ShowDailogueUI(bool show)
    {
        dialoguePanel.SetActive(show);
    }


    /// <summary>
    /// añadimos nombre y imagen al panel
    /// </summary>
    /// <param name="name">nombre del npc</param>
    /// <param name="npcPortrait">imange del npc</param>
    public void SetNPCInfo(string name, Sprite npcPortrait)
    {
        nameText.text = name;
        portraitImage.sprite = npcPortrait;
    }


    /// <summary>
    /// Escribe el dialogo completo en pantalla 
    /// </summary>
    /// <param name="dialogue">dialogo del npc</param>

    public void SetDialogueText(string dialogue)
    {
        dialogueText.text = dialogue;
    }


    /// <summary>
    /// añadimos la siguiente letra al dialogo en pantalla 
    /// </summary>
    /// <param name="nextChar">Siguiente letra a aparecer</param>


    public void SetDialogueText(char nextChar)
    {
        dialogueText.text += nextChar;
    }


    /// <summary>
    /// Resetea el texto de dialogo al string vacio
    /// </summary>
    public void SetDialogueText()
    {
        dialogueText.text = "";
    }

    public void EndDialogue()
    {
        npc.EndDialogue();
    }

    public void ClearChoices()
    {
        foreach(Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void CreatreChoiceButton(string choiceText,UnityAction onClick)
    {
        GameObject choiceButton = Instantiate(choiceButtonPrefab, choicesContainer);
        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText;
        choiceButton.GetComponent<Button>().onClick.AddListener(onClick);
    }



}
