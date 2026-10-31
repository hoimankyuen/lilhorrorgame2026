using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    [Header("Components")] 
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private TMP_Text m_titleText;
    [SerializeField] private TMP_Text m_subTitleText;
    [SerializeField] private Button m_retryButton;
    [SerializeField] private Button m_menuButton;

    // ======== Unity Events ========

    private void Awake()
    {
        m_retryButton.onClick.AddListener(Retry);
        m_menuButton.onClick.AddListener(Menu);
    }
    
    // ======== Menu Functions ========
    
    public void Setup(string title, string subTitle)
    {
        m_titleText.text = title;
        m_subTitleText.text = subTitle;
    }

    public void Show()
    {
        m_canvasGroup.DOFade(1f, 0.5f);
        m_canvasGroup.interactable = true;
        m_canvasGroup.blocksRaycasts = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Retry()
    {
       LevelManager.Instance.GotoGame(); 
    }

    private void Menu()
    {
        LevelManager.Instance.GotoMainMenu(); 

    }
}
