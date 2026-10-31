using UnityEngine;

public class ElevatorDialogue : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider m_collider;
    [SerializeField] private DialogueDisplay m_dialogueDisplay;
    [SerializeField] private TimeManager m_timeManager;

    private bool m_workShiftComplete = false;
    
    private void Start()
    {
        m_timeManager.OnSunset += WorkShiftComplete;
        m_collider.enabled = false;
        Invoke(nameof(ActivateCollider), 30f);
    }

    private void OnDestroy()
    {
        m_timeManager.OnSunset -= WorkShiftComplete;
    }

    private void ActivateCollider()
    {
        m_collider.enabled = true;
    }

    private void WorkShiftComplete()
    {
        m_workShiftComplete = true;
        m_dialogueDisplay.ShowDialogue("My work shift's done. I can head to the elevator now!", 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !m_workShiftComplete)
        {
            m_dialogueDisplay.ShowDialogue("Elevator's not arrived yet. It'll be here after my shift ends at 5pm.");        
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !m_workShiftComplete)
        {
            m_dialogueDisplay.ShowDialogue("");
        }
    }
}