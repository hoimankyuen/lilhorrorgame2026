using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody m_rb;
    [SerializeField] private TimeManager m_timeManager;
    
    private void Start()
    {
        Reset();
    }

    private void OnEnable()
    {
        m_timeManager.OnSunset += UnlockExitDoor;
    }

    private void OnDisable()
    {
        m_timeManager.OnSunset -= UnlockExitDoor;
    }

    private void Reset()
    {
        m_timeManager.OnSunset -= UnlockExitDoor;
        m_timeManager.OnSunset += UnlockExitDoor;
        m_rb.isKinematic = true;
    }

    private void UnlockExitDoor()
    {
        m_rb.isKinematic = false;
    }
}
