using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private HingeJoint m_hinge;
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioClip m_slam;
    [SerializeField] private AudioClip m_gentle;

    [Header("Tuning")]
    [Tooltip("Degrees from the rest angle before the door counts as 'opening'")]
    [SerializeField] float openAngle = 3f;
    [Tooltip("Degrees from rest the door must return within to count as closed again")]
    [SerializeField] float closedAngle = 1f;
    [Tooltip("Angular speed (deg/sec) above which it counts as a slam")]
    [SerializeField] float slamSpeed = 45f;

    HingeJoint hinge;
    bool isClosed = true;
    
    void FixedUpdate()
    {
        float angle = Mathf.Abs(m_hinge.angle);

        if (isClosed && angle > openAngle)
        {
            isClosed = false;
            float speed = Mathf.Abs(m_hinge.velocity);

            m_audioSource.resource = speed >= slamSpeed ? m_slam : m_gentle;
            m_audioSource.Play();
        }
        else if (!isClosed && angle < closedAngle)
        {
            isClosed = true; // re-arm for the next opening
        }
    }
}
