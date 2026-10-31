using System;
using UnityEngine;

public class MonsterSFX : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Monster m_monster;
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private Animator m_animator;
    
    [SerializeField] private AudioClip m_attack;
    [SerializeField] private AudioClip m_roar;
    [SerializeField] private AudioClip m_alert;
    [SerializeField] private AudioClip m_caution;

    private void Start()
    {
        m_monster.CurrentHostilityLevelChanged += CurrentHostilityLevelChanged;
    }

    private void CurrentHostilityLevelChanged(HostileLevel obj)
    {
        switch (obj)
        {
            case HostileLevel.Alert:
                Alert();
                break;
            case HostileLevel.Caution:
                Caution();
                break;
        }
    }

    public void Attack()
    {
        m_audioSource.Stop();
        m_audioSource.PlayOneShot(m_attack);
    }

    public void Roar()
    {
        m_audioSource.Stop();
        m_audioSource.PlayOneShot(m_roar);
    }
    
    private void Alert()
    {
        m_audioSource.Stop();
        m_audioSource.PlayOneShot(m_alert);
    }
    
    private void Caution()
    {
        m_audioSource.Stop();
        m_audioSource.PlayOneShot(m_caution);
    }
}
