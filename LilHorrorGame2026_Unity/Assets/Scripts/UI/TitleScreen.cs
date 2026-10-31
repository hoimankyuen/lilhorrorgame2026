using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    [Header("Components")] 
    [SerializeField] private Button m_playButton;
    [SerializeField] private Button m_creditButton;
    
    // ======== Unity Events ========

    private void Awake()
    {
        m_playButton.onClick.AddListener(Play);
        m_creditButton.onClick.AddListener(Credit);
    }
    
    // ======== Menu Functions ========
    
    private void Play()
    {
        LevelManager.Instance.GotoGame(); 
    }

    private void Credit()
    {
        
    }
}
