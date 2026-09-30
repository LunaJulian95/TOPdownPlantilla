using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour, Iinteractable
{
    [Header("Dialogue")]
    public NPCDialogue dialogueData;

    [Header("Merchant")]
    public Merchant merchant;

    private int dialogueIndex;
    private bool isTipyng;
    private bool isDialogueActive;


    public bool CanInteract()
    {
        return !isDialogueActive;
    }


    public void Interact()
    {
        if (dialogueData == null ||
            (PauseController.IsGamePaused && !isDialogueActive))
        {
            return;
        }

        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }


    private void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        DialogueController.Instance.npc = this;

        DialogueController.Instance.SetNPCInfo(
            dialogueData.npcName,
            dialogueData.npcPortrait
        );

        DialogueController.Instance.ShowDailogueUI(true);

        PauseController.SetPause(true);

        DisplayCurrentLine();
    }


    private void NextLine()
    {
        // Si la línea todavía se está escribiendo,
        // mostramos inmediatamente la línea completa.
        if (isTipyng)
        {
            StopAllCoroutines();

            DialogueController.Instance.SetDialogueText(
                dialogueData.dialogueLines[dialogueIndex]
            );

            isTipyng = false;
            return;
        }


        DialogueController.Instance.ClearChoices();


        // Comprobar si esta línea termina el diálogo.
        if (dialogueData.endDialogueLine.Length > dialogueIndex &&
            dialogueData.endDialogueLine[dialogueIndex])
        {
            EndDialogue();
            return;
        }


        // Buscar si esta línea tiene opciones.
        foreach (DialogueChoice dialogueChoice
                 in dialogueData.dialogueChoices)
        {
            if (dialogueChoice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(dialogueChoice);
                return;
            }
        }


        // Pasar a la siguiente línea.
        dialogueIndex++;

        if (dialogueIndex < dialogueData.dialogueLines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }


    private void DisplayChoices(DialogueChoice choice)
    {
        if (choice.dialogueChoices == null)
            return;

        for (int i = 0; i < choice.dialogueChoices.Length; i++)
        {
            string choiceText = choice.dialogueChoices[i];

            DialogueChoice.ChoiceType choiceType =
                DialogueChoice.ChoiceType.Dialogue;

            if (choice.choiceTypes != null &&
                i < choice.choiceTypes.Length)
            {
                choiceType = choice.choiceTypes[i];
            }


            int nextIndex = -1;

            if (choice.nextDialogueIndex != null &&
                i < choice.nextDialogueIndex.Length)
            {
                nextIndex = choice.nextDialogueIndex[i];
            }


            switch (choiceType)
            {
                case DialogueChoice.ChoiceType.Dialogue:

                    DialogueController.Instance.CreatreChoiceButton(
                        choiceText,
                        () => ChooseChoice(nextIndex)
                    );

                    break;


                case DialogueChoice.ChoiceType.Buy:

                    DialogueController.Instance.CreatreChoiceButton(
                        choiceText,
                        OpenShop
                    );

                    break;


                case DialogueChoice.ChoiceType.Sell:

                    DialogueController.Instance.CreatreChoiceButton(
                        choiceText,
                        OpenShopSell
                    );

                    break;


                case DialogueChoice.ChoiceType.Exit:

                    DialogueController.Instance.CreatreChoiceButton(
                        choiceText,
                        EndDialogue
                    );

                    break;
            }
        }
    }


    private void ChooseChoice(int nextIndex)
    {
        if (nextIndex < 0 ||
            nextIndex >= dialogueData.dialogueLines.Length)
        {
            Debug.LogWarning(
                "El índice de diálogo no es válido: " +
                nextIndex
            );

            EndDialogue();
            return;
        }

        dialogueIndex = nextIndex;

        DialogueController.Instance.ClearChoices();

        DisplayCurrentLine();
    }


    public void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }


    public void EndDialogue()
    {
        StopAllCoroutines();

        isTipyng = false;
        isDialogueActive = false;

        DialogueController.Instance.ClearChoices();

        DialogueController.Instance.npc = null;

        DialogueController.Instance.SetDialogueText();

        DialogueController.Instance.ShowDailogueUI(false);

        PauseController.SetPause(false);
    }


    // =========================================================
    // COMERCIO
    // =========================================================

    public void OpenShop()
    {
        if (merchant == null)
        {
            Debug.LogWarning(
                "Este NPC no tiene Merchant."
            );

            return;
        }

        StopAllCoroutines();

        isTipyng = false;
        isDialogueActive = false;

        DialogueController.Instance.ClearChoices();

        DialogueController.Instance.ShowDailogueUI(false);

        if (ShopController.Instance == null)
        {
            Debug.LogError(
                "No existe un ShopController en la escena."
            );

            return;
        }

        ShopController.Instance.OpenShop(merchant);
    }


    public void OpenShopSell()
    {
        if (merchant == null)
        {
            Debug.LogWarning(
                "Este NPC no tiene Merchant."
            );

            return;
        }

        StopAllCoroutines();

        isTipyng = false;
        isDialogueActive = false;

        DialogueController.Instance.ClearChoices();

        DialogueController.Instance.ShowDailogueUI(false);

        if (ShopController.Instance == null)
        {
            Debug.LogError(
                "No existe un ShopController en la escena."
            );

            return;
        }

        ShopController.Instance.OpenShop(merchant);

        ShopController.Instance.ShowSellPanel();
    }


    public void CloseShop()
    {
        if (ShopController.Instance != null)
        {
            ShopController.Instance.CloseShop();
        }

        PauseController.SetPause(false);
    }


    // =========================================================
    // TYPING
    // =========================================================

    private IEnumerator TypeLine()
    {
        isTipyng = true;

        DialogueController.Instance.SetDialogueText();


        foreach (char c in dialogueData.dialogueLines[dialogueIndex])
        {
            DialogueController.Instance.SetDialogueText(c);

            if (SoundEffectManager.Instance != null)
            {
                SoundEffectManager.Instance.PlayVoice(
                    dialogueData.voiceSound,
                    dialogueData.voicePitch
                );
            }

            yield return new WaitForSeconds(
                dialogueData.tipingSpeed
            );
        }


        isTipyng = false;


        // Auto avanzar.
        if (dialogueData.autoProgressLines.Length > dialogueIndex &&
            dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(
                dialogueData.autoProgressDelay
            );

            NextLine();
        }
    }
}
