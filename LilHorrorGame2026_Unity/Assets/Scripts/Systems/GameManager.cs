using System;
using MoonlightTools.GeneralTools;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [Header("References")]
    [SerializeField] private TimeManager m_timeManager;
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
    
    private void WinGame()
    {
        Debug.Log("Win Game");
        GameEnded?.Invoke();
        GameWon?.Invoke();
        m_audioSource.PlayOneShot(m_winStinger);
        
        //Show win UI
        //Kill monster?
        //Brighten scene?
    }

    public void LoseGameByMonster()
    {
        Debug.Log("Lose Game");
        GameEnded?.Invoke();
        GameLostByMonster?.Invoke();
        m_audioSource.PlayOneShot(m_loseStinger);
        //Show loss UI
        //Monster roar
        //Fade to black?
    }
    
    public void LoseGameBySanity()
    {
        Debug.Log("Lose Game");
        GameEnded?.Invoke();
        GameLostBySanity?.Invoke();
        m_audioSource.PlayOneShot(m_loseStinger);
        //Show loss UI
        //Monster roar
        //Fade to black?
    }
}
