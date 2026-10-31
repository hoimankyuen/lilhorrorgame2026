using System;
using MoonlightTools.GeneralTools;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [Header("References")] 
    [SerializeField] private Cutscene m_winCutscene;
    [SerializeField] private Cutscene m_lostByMonsterCutscene;
    [SerializeField] private Cutscene m_lostBySanityCutscene;
    
    [Space]
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioClip m_winStinger;
    [SerializeField] private AudioClip m_loseStinger;

    public event Action GameEnded;
    public event Action GameWon;
    public event Action GameLostByMonster;
    public event Action GameLostBySanity;
    
    private void Start()
    {
        Application.targetFrameRate = 60;
    }
    
    public void WinGame()
    {
        Debug.Log("Win Game");
        GameEnded?.Invoke();
        GameWon?.Invoke();

        m_winCutscene.PlayCutscene();
        m_audioSource.PlayOneShot(m_winStinger);
    }

    public void LoseGameByMonster()
    {
        Debug.Log("Lose Game");
        GameEnded?.Invoke();
        GameLostByMonster?.Invoke();
        
        m_lostByMonsterCutscene.PlayCutscene();
        m_audioSource.PlayOneShot(m_loseStinger);
    }
    
    public void LoseGameBySanity()
    {
        Debug.Log("Lose Game");
        GameEnded?.Invoke();
        GameLostBySanity?.Invoke();
        
        m_lostBySanityCutscene.PlayCutscene();
        m_audioSource.PlayOneShot(m_loseStinger);
    }
}
