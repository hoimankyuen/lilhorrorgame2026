using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class InteractableLure : Lure, IInteractableComponent
{
    [SerializeField] private float m_duration;
    [SerializeField] private float m_cooldown;
    
    private Interactable m_interactable;
    
    // ======== Unity Events ========

    private void Awake()
    {
        m_interactable.GetComponent<Interactable>();
    }
    
    // ======== Player Interactions =======
    
    public void OnSelected(bool selected)
    {
    }
    
    public void OnInteract()
    {
        StartCoroutine(BroadcastSequence());
    }
    
    private IEnumerator BroadcastSequence()
    {
        Broadcast(true);
        m_interactable.SetAllowInteraction(false);
        yield return m_duration;
        
        Broadcast(false);
        yield return Mathf.Max(m_duration - m_duration, 0f);
        
        m_interactable.SetAllowInteraction(true);
    }
}
