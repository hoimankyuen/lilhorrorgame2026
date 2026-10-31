using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class TitleScreenObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioResource m_audioResource;

    [SerializeField] private bool m_loopAnimation;

    private void Start()
    {
        Invoke(nameof(PlayAnimation), Random.Range(1f, 30f));
    }

    private void PlayAnimation()
    {
        m_animator.SetBool("Active", true);
        m_audioSource.resource = m_audioResource;
        m_audioSource.Play();
    }
}