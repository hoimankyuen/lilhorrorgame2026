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
    
    public static event Action OnWinGame;
    public static event Action OnLoseGame;
    
    private void Start()
    {
        Application.targetFrameRate = 60;
    }
    
    private void WinGame()
    {
        Debug.Log("Win Game");
        OnWinGame?.Invoke();
        m_audioSource.PlayOneShot(m_winStinger);
        
        //Show win UI
        //Kill monster?
        //Brighten scene?
    }

    public void LoseGame()
    {
        Debug.Log("Lose Game");
        OnLoseGame?.Invoke();
        m_audioSource.PlayOneShot(m_loseStinger);
        //Show loss UI
        //Monster roar
        //Fade to black?
    }
}
