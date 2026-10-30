using MoonlightTools.MathTools;
using UnityEngine;

public class EyelidOverlay : MonoBehaviour
{
    [Header("Components")] [SerializeField]
    private CanvasGroup m_eyeLidCanvasGroup;

    [SerializeField] private RectTransform m_upperEyelidCurve;
    [SerializeField] private RectTransform m_upperEyelidFill;
    [SerializeField] private RectTransform m_lowerEyelidCurve;
    [SerializeField] private RectTransform m_lowerEyelidFill;
    [SerializeField] private CanvasGroup m_eyeBlockerCanvasGroup;
    
    
    private float m_curveHeight;
    private float m_fillHeight;
    
    // ======== Unity Events ========

    private void Awake()
    {
        RetrieveInformation();
    }
    
    // ======== Behaviour ========

    private void RetrieveInformation()
    {
        m_fillHeight = m_upperEyelidFill.sizeDelta.y;
        m_curveHeight = m_upperEyelidCurve.sizeDelta.y;
    }

    public void ShowEyelidProgress(float interpolate) // 0: open, 1: close
    {
        interpolate = Mathf.Clamp01(interpolate);
        m_eyeLidCanvasGroup.alpha = interpolate < 1f ? 1f : 0f;
        m_upperEyelidCurve.sizeDelta = new Vector2(m_upperEyelidCurve.sizeDelta.x, Mathf.Lerp(m_curveHeight, 0f, interpolate));
        m_upperEyelidFill.sizeDelta = new Vector2(m_upperEyelidFill.sizeDelta.x, Mathf.Lerp(m_fillHeight, m_fillHeight + m_curveHeight, interpolate));
        m_lowerEyelidCurve.sizeDelta = new Vector2(m_lowerEyelidCurve.sizeDelta.x, Mathf.Lerp(m_curveHeight, 0f, interpolate));
        m_lowerEyelidFill.sizeDelta = new Vector2(m_lowerEyelidFill.sizeDelta.x, Mathf.Lerp(m_fillHeight, m_fillHeight + m_curveHeight, interpolate));
    }

    public void ShowAdaptionProgress(float interpolate) // 0: not adapted, 1: adapted
    {
        interpolate = Mathfx.Coserp(0f, 1f, Mathf.Clamp01(interpolate));
        m_eyeBlockerCanvasGroup.alpha = 1f - interpolate;
    }
}
