using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class InteractableProvoker : Provoker, IInteractableComponent
{
    [Header("Interactable Provoker")]
    [SerializeField] private string m_selectionPromptText;
    [SerializeField] private Color m_selectionColour;
    
    private Interactable m_interactable;
    
    // ======== Unity Events ========

    protected override void Awake()
    {
        base.Awake();
        m_interactable = GetComponent<Interactable>();
        m_interactable.SetSelectionPromptText(m_selectionPromptText);
        m_interactable.SetSelectionOutlineColour(m_selectionColour);
        
        Broadcast(true);
    }
    
    // ======== Selection =======
    
    public void OnSelected(bool selected)
    {
    }
    
    // ======== Interaction =======
    
    public void OnInteract()
    {
        Broadcast(false);

    }
    
    // ======== Effects ========

    public override void Broadcast(bool active)
    {
        base.Broadcast(active);
        m_interactable.SetAllowInteraction(active);
    }
}
