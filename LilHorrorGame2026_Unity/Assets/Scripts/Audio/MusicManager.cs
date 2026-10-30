using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private InputSystemFirstPersonControls m_controls;
    
    [Header("References")]
    [SerializeField] private AudioSource m_audioSource;

    [Header("Settings")] 
    [SerializeField] private float m_normalVolume = 1f;
    [SerializeField] private float m_listeningVolume = 0.1f;

    private void Awake()
    {
        m_controls = new InputSystemFirstPersonControls();
    }

    private void OnEnable()
    {
        m_controls.Enable();
    }

    private void OnDisable()
    {
        m_controls.Disable();
    }

    private void Update()
    {
        m_audioSource.volume = m_controls.FPSController.Listen.IsPressed() ? m_listeningVolume : m_normalVolume;
    }
}
