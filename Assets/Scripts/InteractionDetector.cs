using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null; // closest interactable
    public GameObject interactionIcon;

    public GameObject dialoguePanel;

    public GameObject interactButton;

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
            interactButton.SetActive(false);
        }
    }

    public void OnClick()
    {
        interactableInRange?.Interact();
        interactButton.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            interactionIcon.SetActive(true);

            interactButton.SetActive(true);
            // var buttonImage = interactButton.GetComponent<Image>();
            // buttonImage.color = new Color(250, 250, 250);
            // dialoguePanel.SetActive(true);

            GameManager.Instance.activeNpc = other.gameObject;
            Debug.Log($"activeNpc: {GameManager.Instance.activeNpc}");

            // Highlight by changing color
            var spriteRenderer = other.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.yellow;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if (other.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);

            interactButton.SetActive(false);
            // var buttonImage = interactButton.GetComponent<Image>();
            // buttonImage.color = new Color(160, 160, 160);

            dialoguePanel.SetActive(false);
            PauseController.SetPause(false);

            var spriteRenderer = other.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
            }
            GameManager.Instance.activeNpc = null;
        }
    }
}
