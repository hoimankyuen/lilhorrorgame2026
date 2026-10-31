using System;
using UnityEngine;

public class TimeManager : MonoBehaviour {
    [SerializeField] private TimeSettings m_timeSettings;
    
    public event Action OnSunrise {
        add => m_service.OnSunrise += value;
        remove => m_service.OnSunrise -= value;
    }
    
    public event Action OnSunset {
        add => m_service.OnSunset += value;
        remove => m_service.OnSunset -= value;
    }
    
    public event Action OnMinuteChange {
        add => m_service.OnMinuteChange += value;
        remove => m_service.OnMinuteChange -= value;
    }    
    
    public event Action OnHourChange {
        add => m_service.OnHourChange += value;
        remove => m_service.OnHourChange -= value;
    }    

    private TimeService m_service;
    private bool m_timeSuspended;

    private void Awake() {
        m_service = new TimeService(m_timeSettings);
        OnSunrise += () => Debug.Log("Work day starts");
        OnSunset += () => Debug.Log("Work day ends");
        OnHourChange += () => Debug.Log("Hour change");
    }
    
    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameEnded += OnGameEnded;
        }
    }
    
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameEnded -= OnGameEnded;
        }
    }

    private void Update() {
        UpdateTimeOfDay();
    }

    private void OnGameEnded()
    {
        m_timeSuspended = true;
    }
    
    private void UpdateTimeOfDay()
    {
        if (m_timeSuspended)
            return;
        
        m_service.UpdateTime(Time.deltaTime);

    }

    public DateTime GetTimeOfDay()
    {
        return m_service.CurrentTime;
    }
}