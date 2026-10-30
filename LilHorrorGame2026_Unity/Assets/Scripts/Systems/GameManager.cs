using System;
using MoonlightTools.GeneralTools;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{

    [Header("References")]
    [SerializeField] private TimeManager m_timeManager;
    
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
        //Show win UI
        //Kill monster?
        //Brighten scene?
    }

    public void LoseGame()
    {
        Debug.Log("Lose Game");
        OnLoseGame?.Invoke();
        //Show loss UI
        //Monster roar
        //Fade to black?
    }
}
