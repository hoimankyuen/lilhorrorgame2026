using System;
using UnityEngine;

public class WallClock : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Transform m_minuteHand;
    [SerializeField] private Transform m_hourHand;
    [SerializeField] private TimeManager m_timeManager;
    [SerializeField] private float m_minuteHandOffset;
    [SerializeField] private float m_hourHandOffset;

    private void Start()
    {
        UnsubscribeEvents();
        SubscribeEvents();
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void OnMinuteChanged()
    {
        DateTime currentTime = m_timeManager.GetTimeOfDay();
        
        float minutes = currentTime.Minute;
        float hours = currentTime.Hour;
        
        Debug.Log("Hour: " + hours + " Minutes: " + minutes);
        m_minuteHand.localRotation = Quaternion.Euler(m_minuteHand.rotation.x, m_minuteHand.rotation.y, minutes * 6f);

        float targetRot = hours * (360f / 12f) - m_hourHandOffset;
        
        m_hourHand.localRotation = Quaternion.Euler(m_hourHand.rotation.x, m_hourHand.rotation.y, targetRot);
    }

    private void OnHourChanged()
    {
        
    }

    private void SubscribeEvents()
    {
        m_timeManager.OnMinuteChange += OnMinuteChanged;
        m_timeManager.OnHourChange += OnHourChanged;
    }
    
    private void UnsubscribeEvents()
    {
        m_timeManager.OnMinuteChange -= OnMinuteChanged;
        m_timeManager.OnHourChange -= OnHourChanged;
    }
}