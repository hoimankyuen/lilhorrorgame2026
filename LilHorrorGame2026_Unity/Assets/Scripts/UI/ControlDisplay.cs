using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class ControlDisplay : MonoBehaviour
{
    [System.Serializable]
    public struct ControlData
    {
        public Image Frame;
        public TMP_Text Text;
        
        [Space]
        public Sprite KeyboardSprite;
        public string KeyboardText;
        public Sprite GamepadSprite;
        public string GamepadText;

        public void ChangeDisplay(bool isGamepad)
        {
            Frame.sprite = isGamepad ? GamepadSprite : KeyboardSprite;
            Text.text = isGamepad ? GamepadText : KeyboardText;
        }
    }
    
    [Header("Components")]
    [SerializeField] private CanvasGroup m_mainCanvasGroup;
    
    [Header("Data")]
    [SerializeField] private ControlData m_crouchData;
    [SerializeField] private ControlData m_runData;
    [SerializeField] private ControlData m_listenData;
    [SerializeField] private ControlData m_zoomData;
    
    private bool m_isGamepad;
    
    // ======== Unity Events ========
    
    private void Start()
    {
        InputSystem.onEvent += OnEvent;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameEnded += OnGameEnded;
        }
    }

    private void OnDestroy()
    {
        InputSystem.onEvent -= OnEvent;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameEnded -= OnGameEnded;
        }
    }
    
    // ======== Input System Events ========
    
    private void OnEvent(InputEventPtr ptr, InputDevice device)
    {
        bool isGamePad = device is Gamepad;
        if (isGamePad == m_isGamepad)
            return;
        m_isGamepad = isGamePad;
        
        ChangeDisplay(m_isGamepad);
    }

    // ======== Game Manager Events ========
    
    private void OnGameEnded()
    {
        m_mainCanvasGroup.DOFade(0, 0.5f);
    }
    
    // ======== Appearance ========
    
    private void ChangeDisplay(bool isGamepad)
    {
        m_crouchData.ChangeDisplay(isGamepad);
        m_runData.ChangeDisplay(isGamepad);
        m_listenData.ChangeDisplay(isGamepad);
        m_zoomData.ChangeDisplay(isGamepad);
    }
}
