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
    [SerializeField] private CanvasGroup m_canvasGroup;
    
    [Header("Data")]
    [SerializeField] private ControlData m_crouchData;
    [SerializeField] private ControlData m_runData;
    [SerializeField] private ControlData m_listenData;
    [SerializeField] private ControlData m_zoomData;

    //[Header("Settings")]
    //[SerializeField] private float m_hideDelay;
    //[SerializeField] private float m_hideDuration;
    
    private bool m_isGamepad;
    //private bool m_isHidden;
    
    // ======== Unity Events ========

    private void Awake()
    {
        InputSystem.onEvent += OnEvent;
    }
    
    private void OnDestroy()
    {
        InputSystem.onEvent -= OnEvent;
    }
    
    private void OnEvent(InputEventPtr ptr, InputDevice device)
    {
        bool isGamePad = device is Gamepad;
        if (isGamePad == m_isGamepad)
            return;
        m_isGamepad = isGamePad;
        
        ChangeDisplay(m_isGamepad);
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
