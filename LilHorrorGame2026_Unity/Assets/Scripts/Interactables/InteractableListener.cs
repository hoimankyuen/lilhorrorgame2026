using System.Runtime.CompilerServices;
using MoonlightTools.MathTools;
using UnityEngine;

public class InteractableListener : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputSystemFirstPersonCharacter m_fpCharacter;
    [SerializeField] private Camera m_camera;
    [SerializeField] private EyelidOverlay m_eyelidOverlay;
    
    [Header("Settings")]
    [SerializeField] private RangedFloat m_eyeLidDuration;
    [SerializeField] private RangedFloat m_darknessDuration;
    [SerializeField] private RangedFloat m_adaptionDuration;
    
    [Space]
    [SerializeField] private LayerMask m_layersToHide;
    [SerializeField] private LayerMask m_layersToShow;
    
    private InputSystemFirstPersonControls InputActions => m_fpCharacter != null ? m_fpCharacter.InputActions : null;

    private float m_curveHeight;
    private float m_fillHeight;

    private float m_progress; // 0: open, 1: closed 2: not adapted, 3: adapted
    private float m_prevProgress;
    //private float m_eyeClosingProgress = 0;
    //private float m_lastEyeClosingProgress = 0;
    
    // ======== Unity Events ========
    private void Update()
    {
        DetectEyeClosingProgress();
        ShowAsEyeClosing();
    }
    
    // ======== Detect Input ========

    private float GetInterpolateSpeed(RangedFloat duration, bool isPositive)
    {
        if (isPositive)
        {        
            return duration.max != 0 ? 1f / duration.max : 1000000f;
        }
        else
        {
            return duration.min != 0 ? 1f/ -duration.min : -1000000f;
        }
    }
    
    private void DetectEyeClosingProgress()
    {
        if (InputActions == null)
            return;
        
        bool isPositive = InputActions.FPSController.Listen.ReadValue<float>() > 0.5f;
        m_progress = m_progress switch
        {
            < 0 when isPositive => 0,
            <= 0 when !isPositive => 0,
            >= 0 and < 1 when isPositive => Mathf.Clamp(m_progress + GetInterpolateSpeed(m_eyeLidDuration, true) * Time.deltaTime, 0f, 1f),
            > 0 and <= 1 when !isPositive => Mathf.Clamp(m_progress + GetInterpolateSpeed(m_eyeLidDuration, false) * Time.deltaTime, 0f, 1f),
            >= 1 and < 2 when isPositive => Mathf.Clamp(m_progress + GetInterpolateSpeed(m_eyeLidDuration, true) * Time.deltaTime, 1f, 2f),
            > 1 and <= 2 when !isPositive => Mathf.Clamp(m_progress + GetInterpolateSpeed(m_eyeLidDuration, false) * Time.deltaTime, 1f, 2f),
            >= 2 and < 3 when isPositive => Mathf.Clamp(m_progress + GetInterpolateSpeed(m_adaptionDuration, true) * Time.deltaTime, 2f, 3f),
            > 2 and <= 3 when !isPositive => Mathf.Clamp(m_progress + GetInterpolateSpeed(m_adaptionDuration, false) * Time.deltaTime, 2f, 3f),
            _ => 3
        };
    }
    
    // ======== Behaviour ========
    
    private void ShowAsEyeClosing()
    {
        if (Mathf.Approximately(m_progress, m_prevProgress))
            return;

        // hide or show layers
        if (m_progress > 1f && m_prevProgress <= 1f)
        {
            m_camera.cullingMask = m_camera.cullingMask & ~m_layersToHide | m_layersToShow;
        }
        else if (m_prevProgress > 1f && m_progress <= 1f)
        {
            m_camera.cullingMask = m_camera.cullingMask & ~m_layersToShow | m_layersToHide;
        }
        m_prevProgress = m_progress;
        
        // eyelid animations
        m_eyelidOverlay.ShowEyelidProgress(Mathf.InverseLerp(0f ,1f, m_progress));
        m_eyelidOverlay.ShowAdaptionProgress(m_progress >= 1f ? Mathf.InverseLerp(2f ,3f, m_progress) : 1f);
    }
}
