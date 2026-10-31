using System.Collections.Generic;
using DG.Tweening;
using MoonlightTools.MathTools;
using UnityEngine;
using UnityEngine.UI;

public class WarningDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform m_playerTransform;
    [SerializeField] private Monster m_monster;
    
    [Header("Components")] 
    [SerializeField] private CanvasGroup m_mainCanvasGroup;
    [SerializeField] private CanvasGroup m_allWarningCanvasGroup;
    [SerializeField] private CanvasGroup m_leftCanvasGroup;
    [SerializeField] private CanvasGroup m_rightCanvasGroup;
    [SerializeField] private CanvasGroup m_backCanvasGroup;
    [SerializeField] private List<Image> m_colouredImages;

    [Header("Settings")] 
    [SerializeField] private float m_sideAngleThreshold;
    [SerializeField] private float m_backwardAngleThreshold;
    
    [Space]
    [SerializeField] private Color m_cautionColour;
    [SerializeField] private Color m_alertColour;
    [SerializeField] private Color m_hostileColour;
    
    [Space]
    [SerializeField] private RangedFloat m_blinkDistances;
    [SerializeField] private RangedFloat m_blinkInterval;
    
    private HostileLevel m_hostileLevel;
    private float m_lastBlinkTime;
    
    // ======== Unity Events ========

    private void Start()
    {
        if (m_monster != null)
        {
            m_monster.CurrentHostilityLevelChanged += SetHostileLevel;
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameEnded += OnGameEnded;
        }
    }
    
    private void OnDestroy()
    {
        if (m_monster != null)
        {
            m_monster.CurrentHostilityLevelChanged -= SetHostileLevel;
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameEnded -= OnGameEnded;
        }
    }
    
    private void Update()
    {
        BlinkWarnings();
        SelectWarningSets();
    }
    
    // ======== Game Manager Events ========

    private void OnGameEnded()
    {
        m_mainCanvasGroup.DOFade(0, 0.5f);
    }
    
    // ======== Appearance ========

    private void SetHostileLevel(HostileLevel hostileLevel)
    {
        m_hostileLevel = hostileLevel;
        foreach (Image image in m_colouredImages)
        {
            image.color = m_hostileLevel switch
            {
                HostileLevel.Caution => m_cautionColour,
                HostileLevel.Alert => m_alertColour,
                HostileLevel.Hostile => m_hostileColour,
                _ => Color.white,
            };
        }
    }

    private bool ShouldShowWarning()
    {
        return m_playerTransform != null && m_playerTransform.gameObject.activeInHierarchy &&
               m_monster != null && m_monster.gameObject.activeInHierarchy &&
               m_hostileLevel != HostileLevel.Calm;
    }

    private void SelectWarningSets()
    {
        if (!ShouldShowWarning())
        {
            m_backCanvasGroup.alpha = 0f;
            m_rightCanvasGroup.alpha = 0f;
            m_leftCanvasGroup.alpha = 0f;
            return;
        }
        
        float angle = Vector3.SignedAngle(m_playerTransform.forward, m_monster.transform.position - m_playerTransform.position, Vector3.up);
        if (angle >= m_backwardAngleThreshold || angle <= -m_backwardAngleThreshold)
        {
            m_backCanvasGroup.alpha = 1f;
            m_rightCanvasGroup.alpha = 0f;
            m_leftCanvasGroup.alpha = 0f;
        }
        else if (angle >= m_sideAngleThreshold)
        {
            m_backCanvasGroup.alpha = 0f;
            m_rightCanvasGroup.alpha = 1f;
            m_leftCanvasGroup.alpha = 0f;
        }
        else if (angle <= -m_sideAngleThreshold)
        {
            m_backCanvasGroup.alpha = 0f;
            m_rightCanvasGroup.alpha = 0f;
            m_leftCanvasGroup.alpha = 1f;
        }
        else
        {
            m_backCanvasGroup.alpha = 0f;
            m_rightCanvasGroup.alpha = 0f;
            m_leftCanvasGroup.alpha = 0f;
        }
    }
    
    private void BlinkWarnings()
    {
        if (!ShouldShowWarning())
        {
            m_allWarningCanvasGroup.alpha = 0f;
            return;
        }

        float distance = Vector3.Distance(m_playerTransform.position, m_monster.transform.position);
        float blinkInterval = m_blinkInterval.Lerp(m_blinkDistances.InverseLerp(distance));
        if (Time.time - m_lastBlinkTime < blinkInterval)
            return;

        m_allWarningCanvasGroup.alpha = m_allWarningCanvasGroup.alpha > 0.5f ? 0f : 1f;
        m_lastBlinkTime = Time.time;
    }
}
