using UnityEngine;

public class MonsterSFX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private Animator m_animator;
    
    [SerializeField] private AudioClip m_roar;
    
    public void Roar()
    {
        m_audioSource.PlayOneShot(m_roar);
    }
}
