using UnityEngine;
using UnityEngine.UI;

public class SanitySlider : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Slider m_slider;
    [SerializeField] private SanitySystem m_sanitySystem;

    private void OnEnable()
    {
        m_sanitySystem.OnSanityChanged += OnSanityChanged;
    }

    private void OnDisable()
    {
        m_sanitySystem.OnSanityChanged -= OnSanityChanged;
    }

    private void OnSanityChanged(float current)
    {
        m_slider.value = current;
    }
}