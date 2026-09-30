using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private Iinteractable interactableInRange = null; // The interactable object that the player is currently interacting with 

    public GameObject interactionIcon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactionIcon.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent(out Iinteractable interactable)&&
            interactable.CanInteract())
        {
            interactableInRange = interactable;
            interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Iinteractable interactable) &&
            interactableInRange == interactable)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }

    public void OnInteract()
    {
        if (interactableInRange == null)
            return;

        interactableInRange.Interact();

        if (!interactableInRange.CanInteract())
        {
            interactionIcon.SetActive(false);
        }
    }


}
