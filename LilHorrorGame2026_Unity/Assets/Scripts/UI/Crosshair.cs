using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private RectTransform m_iconRectTransform;
    [SerializeField] private List<Image> m_iconImages;
    [SerializeField] private TMP_Text m_promptText;

    [Header("Settings")] 
    [SerializeField] private float m_iconSizeDuration;
    [Space]
    [SerializeField] private float m_normalIconWidth;
    [SerializeField] private float m_selectedIconWidth;
    [SerializeField] private Color m_normalIconColour;
    [SerializeField] private Color m_validIconColour;

    private float m_iconHeight;
    private bool m_isSelected;
    
    // ======== Unity Events ========

    private void Awake()
    {
        m_iconHeight = m_iconRectTransform.sizeDelta.y;
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
        foreach (Image iconImage in m_iconImages)
        {
            iconImage.color = info.isValid ? m_validIconColour : m_normalIconColour;
        }
        m_promptText.text = info.prompt;
        m_promptText.color =  info.isValid ? m_validIconColour : m_normalIconColour;
    }
}
