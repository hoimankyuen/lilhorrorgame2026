using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    [Header("Main Page Components")] 
    [SerializeField] private GameObject m_mainPage;
    [SerializeField] private Button m_playButton;
    [SerializeField] private Button m_creditButton;
    [SerializeField] private Button m_quitButton;

    [Header("Credit Page Components")]
    [SerializeField] private GameObject m_creditPage;
    [SerializeField] private CreditEntry m_creditEntryPrefab;
    [SerializeField] private Transform m_creditEntryContainer;
    [SerializeField] private Button m_backButton;

    [Space] 
    [SerializeField] private List<CreditEntryData> m_creditEntryData;
    
    // ======== Unity Events ========

    private void Awake()
    {
        Application.targetFrameRate = 60;
        
        m_playButton.onClick.AddListener(Play);
        m_creditButton.onClick.AddListener(ShowCreditPage);
        m_backButton.onClick.AddListener(ShowMainPage);
        m_quitButton.onClick.AddListener(QuitGame);
    }

    private void Start()
    {
        PopulateCredits();
        ShowMainPage();
    }
    
    // ======== Main Page Functions ========
    
    private void Play()
    {
        LevelManager.Instance.GotoGame(); 
    }

    private void ShowCreditPage()
    {
        m_mainPage.SetActive(false);
        m_creditPage.SetActive(true);
        m_backButton.Select();
    }

    private void QuitGame()
    {
        Application.Quit();   
    }
    
    // ======== Credit Page Functions ========

    private void PopulateCredits()
    {
        foreach (CreditEntryData creditEntryData in m_creditEntryData)
        {
            CreditEntry entry = Instantiate(m_creditEntryPrefab, m_creditEntryContainer);
            entry.Setup(creditEntryData);
        }
        m_creditEntryPrefab.gameObject.SetActive(false);
    }
    
    private void ShowMainPage()
    {
        m_mainPage.SetActive(true);
        m_creditPage.SetActive(false);
        m_playButton.Select();
    }
}
