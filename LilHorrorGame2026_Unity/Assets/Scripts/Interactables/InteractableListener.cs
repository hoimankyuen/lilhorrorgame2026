using MoonlightTools.MathTools;
using UnityEngine;
using UnityEngine.UI;

public class InteractableListener : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputSystemFirstPersonCharacter m_fpCharacter;
    [SerializeField] private Camera m_camera;

    [Space]
    [SerializeField] private CanvasGroup m_eyeLidCanvasGroup;
    [SerializeField] private RectTransform m_upperEyelidCurve;
    [SerializeField] private RectTransform m_upperEyelidFill;
    [SerializeField] private RectTransform m_lowerEyelidCurve;
    [SerializeField] private RectTransform m_lowerEyelidFill;
    [SerializeField] private CanvasGroup m_eyeBlockerCanvasGroup;
    
    [Header("Settings")]
    [SerializeField] private float m_closeEyeDuration = 1.4f;
    [SerializeField] private float m_openEyeDuration = 0.2f;
    [SerializeField, Range(0, 1)] private float m_animationMidway = 0.25f;
    [SerializeField] private LayerMask m_layersToHide;
    [SerializeField] private LayerMask m_layersToShow;
    [SerializeField, Range(0, 1)] private float m_startHearingMidway = 0.33f;
    
    private InputSystemFirstPersonControls InputActions => m_fpCharacter != null ? m_fpCharacter.InputActions : null;

    private float m_curveHeight;
    private float m_fillHeight;
        
    private float m_eyeClosingProgress = 0;
    private float m_lastEyeClosingProgress = 0;
    
    // ======== Unity Events ========

    private void Awake()
    {
        RetrieveInformation();
    }
    
    private void Update()
    {
        DetectEyeClosingProgress();
        ShowAsEyeClosing();
    }
    
    // ======== Detect Input ========

    private void DetectEyeClosingProgress()
    {
        if (InputActions == null)
            return;
        
        bool closingEye = InputActions.FPSController.Listen.ReadValue<float>() > 0.5f;
        if (closingEye)
        {
            m_eyeClosingProgress += 1 / m_closeEyeDuration * Time.deltaTime;
        }
        else
        {
            m_eyeClosingProgress -= 1 / m_openEyeDuration * Time.deltaTime;
        }
        m_eyeClosingProgress = Mathf.Clamp01(m_eyeClosingProgress);
    }
    
    // ======== Behaviour ========

    private void RetrieveInformation()
    {
        m_fillHeight = m_upperEyelidFill.sizeDelta.y;
        m_curveHeight = m_upperEyelidCurve.sizeDelta.y;
    }
    
    private void ShowAsEyeClosing()
    {
        if (Mathf.Approximately(m_eyeClosingProgress, m_lastEyeClosingProgress))
            return;

        // hide or show layers
        if (m_eyeClosingProgress > m_animationMidway && m_lastEyeClosingProgress <= m_animationMidway)
        {
            m_camera.cullingMask &= ~m_layersToHide;
            m_camera.cullingMask |= m_layersToShow;
        }
        else if (m_lastEyeClosingProgress > m_animationMidway && m_eyeClosingProgress <= m_animationMidway)
        {
            m_camera.cullingMask &= ~m_layersToShow;
            m_camera.cullingMask |= m_layersToHide;
        }
        m_lastEyeClosingProgress = m_eyeClosingProgress;
        
        // eyelid animation
        float eyelidInterpolate = Mathf.InverseLerp(0f, m_animationMidway, m_eyeClosingProgress);
        m_eyeLidCanvasGroup.alpha = eyelidInterpolate < 1f ? 1f : 0f;
        m_upperEyelidCurve.sizeDelta = new Vector2(m_upperEyelidCurve.sizeDelta.x, Mathf.Lerp(m_curveHeight, 0f, eyelidInterpolate));
        m_upperEyelidFill.sizeDelta = new Vector2(m_upperEyelidFill.sizeDelta.x, Mathf.Lerp(m_fillHeight, m_fillHeight + m_curveHeight, eyelidInterpolate));
        m_lowerEyelidCurve.sizeDelta = new Vector2(m_lowerEyelidCurve.sizeDelta.x, Mathf.Lerp(m_curveHeight, 0f, eyelidInterpolate));
        m_lowerEyelidFill.sizeDelta = new Vector2(m_lowerEyelidFill.sizeDelta.x, Mathf.Lerp(m_fillHeight, m_fillHeight + m_curveHeight, eyelidInterpolate));
        
        // blocker animation
        float blockerInterpolate = Mathf.InverseLerp(m_animationMidway, 1f, m_eyeClosingProgress);
        if (blockerInterpolate <= 0f)
        {
            m_eyeBlockerCanvasGroup.alpha = 0f;
        }
        else if (blockerInterpolate <= m_startHearingMidway)
        {
            m_eyeBlockerCanvasGroup.alpha = 1f;
        }
        else
        {
            m_eyeBlockerCanvasGroup.alpha = 1f - Mathfx.Coserp(0f, 1f, Mathf.InverseLerp(m_startHearingMidway, 1f, blockerInterpolate));
        }
    }
}
