using UnityEngine;
using UnityEngine.Audio;

public class FootstepAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource m_audioSource;
    
    private InputSystemFirstPersonControls m_controls;

    [Header("Settings")]
    [SerializeField] private float m_stepInterval = 0.45f;
    [SerializeField] private float m_standingFootstepVolume = 1f;
    [SerializeField] private float m_crouchingFootstepVolume = 1f;
    private float m_stepTimer;
    
    private void Awake()
    {
        m_controls = new InputSystemFirstPersonControls();
    }

    private void OnEnable() => m_controls.Enable();
    private void OnDisable() => m_controls.Disable();

    private void Update()
    {
        if (m_controls.FPSController.Move.IsPressed())
        {
            m_audioSource.volume = m_controls.FPSController.Crouch.IsPressed() ? m_crouchingFootstepVolume : m_standingFootstepVolume;

            m_stepTimer -= m_controls.FPSController.Run.IsPressed() ? Time.deltaTime * 2 : Time.deltaTime;

            if (m_stepTimer <= 0f)
            {
                m_audioSource.Play();
                m_stepTimer = m_stepInterval;
            }
        }
        else
        {
            m_stepTimer = 0;
        }
    }
}