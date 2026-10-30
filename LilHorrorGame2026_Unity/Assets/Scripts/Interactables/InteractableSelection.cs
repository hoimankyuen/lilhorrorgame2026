using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableSelection : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_maxInteractionDistance = 2f;
    [SerializeField] private bool m_freeCast;
    
    [Header("References")]
    [SerializeField] private InputSystemFirstPersonCharacter m_fpCharacter;
    [SerializeField] private Camera m_camera;
    [SerializeField] private TMP_Text promptText;
    
    private Interactable m_selectedInteractable;

    private InputSystemFirstPersonControls InputActions => m_fpCharacter != null ? m_fpCharacter.InputActions : null;

    // ======== Unity Events ========
    
    private void Update()
    {
        Ray ray = m_freeCast
            ? m_camera.ScreenPointToRay(Mouse.current.position.value)
            : m_camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
        DetectInteractable(ray);
    }

    // ======== Detection ========
    
    private void DetectInteractable(Ray ray)
    {
        Interactable newSelectedInteractable = null;
        if (Physics.Raycast(ray, out RaycastHit hit, m_maxInteractionDistance))
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
            if (InputActions != null ? InputActions.FPSController.Interact.WasPressedThisFrame() : Mouse.current.leftButton.isPressed)
            {
                m_selectedInteractable.Interact();
                m_selectedInteractable.Select(true);
            }
        }
    }
    
    // ======== Appearance ========

    private void ChangePromptText(string text)
    {
        promptText.text = text;
    }
}