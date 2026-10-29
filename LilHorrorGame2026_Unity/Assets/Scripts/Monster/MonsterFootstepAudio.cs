using UnityEngine;
using UnityEngine.AI;

public class MonsterFootstepAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private NavMeshAgent m_navMeshAgent;

    [Header("Settings")]
    [SerializeField] private float m_stepInterval = 0.45f;
    private float m_stepTimer;

    private void Update()
    {
        if (m_navMeshAgent.remainingDistance > 0)
        {
            m_stepTimer -=  Time.deltaTime;

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
