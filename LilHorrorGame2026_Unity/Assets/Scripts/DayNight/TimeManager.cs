using System;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI m_timeText;
    
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

    private void Awake() {
        m_service = new TimeService(m_timeSettings);
        OnSunrise += () => Debug.Log("Work day starts");
        OnSunset += () => Debug.Log("Work day ends");
        OnHourChange += () => Debug.Log("Hour change");
    }

    private void Update() {
        UpdateTimeOfDay();
        
        if (Input.GetKeyDown(KeyCode.Space)) {
            m_timeSettings.timeMultiplier *= 2;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            m_timeSettings.timeMultiplier /= 2;
        }
    }

    private void UpdateTimeOfDay() {
        m_service.UpdateTime(Time.deltaTime);
        if (m_timeText != null) {
            m_timeText.text = m_service.CurrentTime.ToString("hh:mm");
        }
    }

    public DateTime GetTimeOfDay()
    {
        return m_service.CurrentTime;
    }
}