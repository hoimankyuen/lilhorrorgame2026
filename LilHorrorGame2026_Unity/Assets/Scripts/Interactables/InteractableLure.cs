using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class InteractableLure : Lure, IInteractableComponent
{
    public static event Action<float> OnLureInteracted; 
    
    [Header("Interactable Lure Settings")]
    [SerializeField] private string m_normalSelectionPromptText;
    [SerializeField] private string m_usedSelectionPromptText;
    [SerializeField] private Color m_normalSelectionColour;
    [SerializeField] private Color m_usedSelectionColour;
    [SerializeField] private string m_cooldownAnimationParam;
    
    [Space]
    [SerializeField] private float m_duration;
    [SerializeField] private float m_cooldown;

    [Space] [SerializeField] private float m_sanityAmount = 20f;
    
    private Interactable m_interactable;
    private Coroutine m_broadcastCoroutine;
    
    // ======== Unity Events ========

    protected override void Awake()
    {
        base.Awake();
        m_interactable = GetComponent<Interactable>();
        m_interactable.SetAllowInteraction(true);
        m_interactable.SetSelectionPromptText(m_normalSelectionPromptText);
        m_interactable.SetSelectionOutlineColour(m_normalSelectionColour);
    }
    
    // ======== Selection =======
    
    public void OnSelected(bool selected)
    {
    }
    
    // ======== Interaction =======
    
    public void OnInteract()
    {
        if (m_broadcastCoroutine != null)
            return;
        
        m_broadcastCoroutine = StartCoroutine(BroadcastSequence());
        
        OnLureInteracted?.Invoke(m_sanityAmount);
    }
    
    private IEnumerator BroadcastSequence()
    {
        ChangeActiveState(true);
        m_interactable.SetSelectionPromptText(m_usedSelectionPromptText);
        m_interactable.SetSelectionOutlineColour(m_usedSelectionColour);
        yield return new WaitForSeconds(m_duration);
        
        m_animator.SetBool(m_cooldownAnimationParam, true);
        ChangeActiveState(false);
        yield return new WaitForSeconds(m_cooldown);
        
        m_animator.SetBool(m_cooldownAnimationParam, false);
        m_interactable.SetSelectionPromptText(m_normalSelectionPromptText);
        m_interactable.SetSelectionOutlineColour(m_normalSelectionColour);
        m_broadcastCoroutine = null;
    }
}
