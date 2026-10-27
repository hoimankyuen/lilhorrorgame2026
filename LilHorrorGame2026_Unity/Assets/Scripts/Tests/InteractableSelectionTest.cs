using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableSelectionTest : MonoBehaviour
{
    [SerializeField] private Camera m_camera;
    [SerializeField] private TMP_Text promptText;
    
    private Interactable m_selectedInteractable;
    
    private void Update()
    {
        Ray mouseRay = m_camera.ScreenPointToRay(Mouse.current.position.value);
        DetectInteractable(mouseRay);
    }

    private void DetectInteractable(Ray ray)
    {
        Interactable newSelectedInteractable = null;
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMask.GetMask("Interactables")))
        {
            newSelectedInteractable = hit.collider.GetComponentInParent<Interactable>();
            if (newSelectedInteractable != null && !newSelectedInteractable.AllowInteraction)
            {
                newSelectedInteractable = null;
            }
        }

        if (m_selectedInteractable != newSelectedInteractable)
        {
            if (m_selectedInteractable != null)
            {
                m_selectedInteractable.Select(false);
                m_selectedInteractable.SelectionPromptTextChanged -= ChangePromptText;
            }
            if (newSelectedInteractable != null)
            {
                newSelectedInteractable.Select(true);
                ChangePromptText(newSelectedInteractable.SelectionPromptText);
                newSelectedInteractable.SelectionPromptTextChanged += ChangePromptText;
            }
            else
            {
                ChangePromptText("");
            }
            m_selectedInteractable = newSelectedInteractable;
        }

        if (m_selectedInteractable != null)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                m_selectedInteractable.Interact();
                m_selectedInteractable.Select(true);
            }
        }
    }

    private void ChangePromptText(string text)
    {
        promptText.text = text;
    }
}
