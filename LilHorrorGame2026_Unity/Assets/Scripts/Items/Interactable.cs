using System;
using QuickerEffects;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Outline m_selectionOutline;
    
    private IInteractableComponent m_component;
    
    public bool AllowInteraction { get; private set; }
    
    public bool Selected { get; private set; }
    
    public string SelectionPromptText { get; private set; }
    public event Action<string> SelectionPromptTextChanged;
    
    // ======== Unity Events ========
    
    private void Awake()
    {
        RetrieveComponents();
    }
    
    // ======== Component Retrieval ========

    private void RetrieveComponents()
    {
        m_component = GetComponent<IInteractableComponent>();
    }
    
    // ======== Player Interactions ========

    public void SetAllowInteraction(bool allow)
    {
        AllowInteraction = allow;
        Select(Selected);
    }
    
    public void Select(bool selected)
    {
        if (m_component == null)
            return;
        
        Selected = AllowInteraction && selected;
        m_selectionOutline.Show = Selected;

        if (m_component != null)
        {
            m_component.OnSelected(selected);
        }
    }
    
    public void Interact()
    {
        if (!AllowInteraction)
            return;

        if (m_component != null)
        {
            m_component.OnInteract();
        }
    }
    
    // ======== Appearance ========

    public void SetSelectionOutlineColour(Color colour)
    {
        m_selectionOutline.Color = colour;
    }

    public void SetSelectionPromptText(string promptText)
    {
        SelectionPromptText = promptText;
        SelectionPromptTextChanged?.Invoke(SelectionPromptText);
    }
}
