using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null; // closest interactable
    public GameObject interactionIcon;

    public GameObject dialoguePanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactionIcon.SetActive(false);
    }

   
    public void OnInteract(InputAction.CallbackContext context)
    {

        if (context.performed) // to fire only once
        {
            Debug.Log("InteractionDetector Interact!");
            interactableInRange?.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            interactionIcon.SetActive(true);
            // dialoguePanel.SetActive(true);

            GameManager.Instance.activeNpc = other.gameObject;
            Debug.Log($"activeNpc: {GameManager.Instance.activeNpc}");
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if (other.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
            dialoguePanel.SetActive(false);
            PauseController.SetPause(false);

            GameManager.Instance.activeNpc = null;
        }
    }
}
