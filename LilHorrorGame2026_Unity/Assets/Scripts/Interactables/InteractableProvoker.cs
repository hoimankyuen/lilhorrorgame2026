using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class InteractableProvoker : Provoker, IInteractableComponent
{
    [Header("Interactable Provoker")]
    [SerializeField] private string m_selectionPromptText;
    [SerializeField] private Color m_selectionColour;
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioClip m_activationSound;
    [SerializeField] private AudioClip m_deactivationSound;
    [SerializeField] private bool m_loopActivationSound;
    private Interactable m_interactable;
    
    // ======== Unity Events ========

    private void Awake()
    {
        m_interactable = GetComponent<Interactable>();
        m_interactable.SetSelectionPromptText(m_selectionPromptText);
        m_interactable.SetSelectionOutlineColour(m_selectionColour);
    }
    
    // ======== Selection =======
    
    public void OnSelected(bool selected)
    {
    }
    
    // ======== Interaction =======
    
    public void OnInteract()
    {
        ChangeActiveState(false);
    }
    
    // ======== Effects ========

    public override void ChangeActiveState(bool active)
    {
        base.ChangeActiveState(active);
        m_interactable.SetAllowInteraction(active);

        if (active)
        {
            if (m_loopActivationSound)
            {
                m_audioSource.loop = true;
                m_audioSource.clip = m_activationSound;
                m_audioSource.Play();
            }
            else
            {
                m_audioSource.loop = false;
                m_audioSource?.PlayOneShot(m_activationSound);
            }
        }
        else
        {
            m_audioSource?.PlayOneShot(m_deactivationSound);
        }
        
    }
}
