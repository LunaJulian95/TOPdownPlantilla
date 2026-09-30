using UnityEngine;

[CreateAssetMenu(fileName ="New npc dialogue",menuName ="Scriptable Object/NPC Dialogue",order =0)]

public class NPCDialogue : ScriptableObject
{
    [Header("Npc data")]
    public string npcName;
    public Sprite npcPortrait;

    [Header("Dialogue Setting")]
    public string[] dialogueLines;
    public bool[] autoProgressLines;
    public bool[] endDialogueLine;
    public float autoProgressDelay = 1.5f;
    public float tipingSpeed = 0.05f;

    [Header("Dialogue Choices")]
    public DialogueChoice[] dialogueChoices;

    [Header("Audio Setting")]
    public AudioClip voiceSound;
    public float voicePitch = 1f;

    
}


[System.Serializable]
public class DialogueChoice
{
    public enum ChoiceType
    {
        Dialogue,
        Buy,
        Sell,
        Exit
    }

    [Header("Configuración")]
    public int dialogueIndex;

    public string[] dialogueChoices;

    public int[] nextDialogueIndex;

    public ChoiceType[] choiceTypes;
}
