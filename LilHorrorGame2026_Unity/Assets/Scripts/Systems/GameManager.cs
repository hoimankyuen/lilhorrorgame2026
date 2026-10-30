using System;
using MoonlightTools.GeneralTools;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{

    [Header("References")]
    [SerializeField] private TimeManager m_timeManager;
    
    private void Start()
    {
        m_timeManager.OnSunset += OnSunset;
        Application.targetFrameRate = 60;
    }

    private void OnSunset()
    {
        WinGame();
    }
    
    private void WinGame()
    {
        Debug.Log("Win Game");
        //Show win UI
        //Kill monster?
        //Brighten scene?
    }

    public void LoseGame()
    {
        Debug.Log("Lose Game");
        
        //Show loss UI
        //Monster roar
        //Fade to black?
    }
}
