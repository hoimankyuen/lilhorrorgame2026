using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody m_rb;
    [SerializeField] private TimeManager m_timeManager;
    [SerializeField] private GameObject m_seamBlackout;
    
    private void Start()
    {
        m_timeManager.OnSunset += UnlockExitDoor;
        m_rb.isKinematic = true;
        m_seamBlackout.SetActive(true);
    }

    private void OnDestroy()
    {
        m_timeManager.OnSunset -= UnlockExitDoor;
    }

    private void UnlockExitDoor()
    {
        m_seamBlackout.SetActive(false);
        m_rb.isKinematic = false;
    }
}
