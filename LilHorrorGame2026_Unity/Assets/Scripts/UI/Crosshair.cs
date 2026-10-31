using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private RectTransform m_iconRectTransform;
    [SerializeField] private List<Graphic> m_colouredGraphics;
    [SerializeField] private TMP_Text m_promptText;
    [SerializeField] private GameObject m_buttonBase;
    [SerializeField] private Image m_buttonFrame;
    [SerializeField] private TMP_Text m_buttonText;

    [Header("Settings")] 
    [SerializeField] private float m_iconSizeDuration;
    [Space]
    [SerializeField] private float m_normalIconWidth;
    [SerializeField] private float m_selectedIconWidth;
    [SerializeField] private Color m_normalIconColour;
    [SerializeField] private Color m_validIconColour;

    [Space] 
    [SerializeField] private Sprite m_keyboardButtonSprite;
    [SerializeField] private string m_keyboardButtonText;
    [SerializeField] private Sprite m_gamepadButtonSprite;
    [SerializeField] private string m_gamepadButtonText;
    
    private float m_iconHeight;
    private bool m_isSelected;

    private bool m_isGamepad;
    
    // ======== Unity Events ========

    private void Awake()
    {
        m_iconHeight = m_iconRectTransform.sizeDelta.y;
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
        
        ChangeButton(m_isGamepad);
    }
    
    // ======== Appearance ========

    public void ShowAsSelected(bool isSelected)
    {
        if (isSelected == m_isSelected)
            return;
        m_isSelected = isSelected;
        m_iconRectTransform.DOSizeDelta(new Vector2(m_isSelected ? m_selectedIconWidth : m_normalIconWidth, m_iconHeight), m_iconSizeDuration);
    }

    public void ChangeInfo(InteractableSelectionInfo info)
    {
        foreach (Graphic colouredGraphic in m_colouredGraphics)
        {
            colouredGraphic.color = info.isValid ? m_validIconColour : m_normalIconColour;
        }
        m_promptText.text = info.prompt;
        m_buttonBase.SetActive(info.isValid);
    }

    private void ChangeButton(bool isGamepad)
    {
        m_buttonFrame.sprite = isGamepad ? m_gamepadButtonSprite : m_keyboardButtonSprite;
        m_buttonText.text = isGamepad ? m_gamepadButtonText : m_keyboardButtonText;
    }
}
