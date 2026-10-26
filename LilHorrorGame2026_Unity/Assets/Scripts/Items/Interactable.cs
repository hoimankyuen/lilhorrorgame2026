using System.Collections.Generic;
using QuickerEffects;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private Outline m_highlightOutline;
    
    private readonly List<IInteractableComponent> m_components = new();
    
    public bool AllowInteraction { get; private set; }
    
    public bool Selected { get; private set; }
    
    // ======== Unity Events ========
    
    private void Awake()
    {
        RetrieveComponents();
    }
    
    // ======== Component Retrieval ========

    private void RetrieveComponents()
    {
        m_components.AddRange(GetComponentsInChildren<IInteractableComponent>());
    }
    
    // ======== Player Interactions ========

    public void SetAllowInteraction(bool allow)
    {
        AllowInteraction = allow;
        m_highlightOutline.Show = AllowInteraction;

        if (!AllowInteraction)
        {
            Select(false);
        }
    }
    
    public void Select(bool selected)
    {
        Selected = selected;
        foreach (IInteractableComponent component in m_components)
        {
            component.OnSelected(selected);
        }
    }
    
    public void Interact()
    {
        foreach (IInteractableComponent component in m_components)
        {
            component.OnInteract();
        }
    }
}
