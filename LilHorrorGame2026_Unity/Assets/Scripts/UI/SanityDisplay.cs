using System.Collections.Generic;
using DG.Tweening;
using MoonlightTools.MathTools;
using UnityEngine;
using UnityEngine.UI;

public class SanityDisplay : MonoBehaviour
{
    [System.Serializable]
    public struct IrisMovementSettings
    {
        public RangedFloat AmountRange;
        
        [Space]
        public float Radius;
        public float MovementDuration;
        
        [Space]
        public float CenteringChance;
        public RangedFloat StayDuration;
        
        [Space]
        public Sprite EyeFrameSprite;
        public Sprite EyeMaskSprite;
        public Sprite EyeIrisSprite;
        public Color FillBarColour;
    }
    
    [Header("References")] 
    [SerializeField] private SanitySystem m_sanitySystem;
    
    [Header("Components")]
    [SerializeField] private CanvasGroup m_mainCanvasGroup;
    [SerializeField] private Image m_eyeFrame;
    [SerializeField] private Image m_eyeMask;
    [SerializeField] private Image m_eyeIris;

    [Space] 
    [SerializeField] private List<Image> m_fills;

    [Header("Settings")]
    [SerializeField] private List<IrisMovementSettings> m_irisMovementSettings;
    
    private IrisMovementSettings m_settings;
    private float m_nextActionTime;
    
    // ======== Unity Events ========
    
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameEnded -= OnGameEnded;
        }
        if (m_sanitySystem != null)
        {
            m_sanitySystem.OnSanityChanged -= OnSanityChanged;
        }
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameEnded += OnGameEnded;
        }
        if (m_sanitySystem != null)
        {
            m_sanitySystem.OnSanityChanged += OnSanityChanged;
        }
    }

    private void Update()
    {
        EyeAnimation();
    }
    
    // ======== Game Manager Events ========
    
    private void OnGameEnded()
    {
        m_mainCanvasGroup.DOFade(0, 0.5f);
    }
    
    // ======== Appearance ========

    private void OnSanityChanged(float amount)
    {
        m_settings = GetApplicableSettings(amount);
        foreach (Image fill in m_fills)
        {
            fill.DOFillAmount(amount / 100f, 0.5f);
            fill.color = m_settings.FillBarColour;
        }
    }
    
    private void EyeAnimation()
    {
        if (Time.time < m_nextActionTime)
            return;
        
        m_eyeFrame.sprite = m_settings.EyeFrameSprite;
        m_eyeMask.sprite = m_settings.EyeMaskSprite;
        m_eyeIris.sprite = m_settings.EyeIrisSprite;
        
        bool isCentred = Random.Range(0f, 1f) < m_settings.CenteringChance;
        Vector2 targetPosition = isCentred ? Vector2.zero : Random.insideUnitCircle * m_settings.Radius;
        m_eyeIris.rectTransform.DOAnchorPos(targetPosition, m_settings.MovementDuration);
        
        m_nextActionTime = Time.time + m_settings.StayDuration.Random();
    }

    private IrisMovementSettings GetApplicableSettings(float amount)
    {
        foreach (IrisMovementSettings movementSettings in m_irisMovementSettings)
        {
            if (movementSettings.AmountRange.Contains(amount))
            {
                return movementSettings;
            }
        }
        return m_irisMovementSettings[0];
    }
}
