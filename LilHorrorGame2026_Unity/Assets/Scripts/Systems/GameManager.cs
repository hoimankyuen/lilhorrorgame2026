using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("References")]
    [SerializeField] private TimeManager m_timeManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        m_timeManager.OnSunset += OnSunset;
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
