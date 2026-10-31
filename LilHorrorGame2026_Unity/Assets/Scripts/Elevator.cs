using System;
using TMPro;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TimeManager m_timeManager;
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private Animator m_animator;
    [SerializeField] private AudioClip m_arrivalDing;
    [SerializeField] private AudioClip m_openDoors;
    [SerializeField] private AudioClip m_closeDoors;
    [SerializeField] private TextMeshPro m_indicator;
    
    private void Start()
    {
        m_timeManager.OnSunset += OpenDoors;
    }

    private void OnDestroy()
    {
        m_timeManager.OnSunset -= OpenDoors;
    }

    private void OpenDoors()
    {
        m_indicator.SetText("O");
        m_indicator.color = Color.green;
        m_audioSource.PlayOneShot(m_arrivalDing);
        m_animator.Play("Open");
    }

    private void CloseDoors()
    {
        m_audioSource.Play();
        m_animator.Play("Close");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CloseDoors();
        }
    }
}