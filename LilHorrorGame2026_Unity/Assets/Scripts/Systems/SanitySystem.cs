using System;
using UnityEngine;

public class SanitySystem : MonoBehaviour
{
    public event Action<float> OnSanityChanged;
    
    [Header("Settings")] 
    [SerializeField] private float m_maxSanity;
    [SerializeField] private float m_decayRate;
    [SerializeField] private float m_decayTickAmount;
    [SerializeField] private float m_initialTickDelay = 5f;
    
    [Header("Stats")]
    [SerializeField] private float m_currentSanity;
    
    private void Start()
    {
        Reset();
    }

    private void OnEnable()
    {
        Provoker.BroadcastingProvokerAdded += OnProvokerAdded;
        Provoker.BroadcastingProvokerRemoved += OnProvokerRemoved;
        Provoker.BroadcastingProvokerChanged += OnProvokersChanged;
    }

    private void OnDisable()
    {
        Provoker.BroadcastingProvokerAdded -= OnProvokerAdded;
        Provoker.BroadcastingProvokerRemoved -= OnProvokerRemoved;
        Provoker.BroadcastingProvokerChanged -= OnProvokersChanged;
    }

    private void OnProvokerAdded(float amount)
    {
        ChangeSanity(amount, false);
    }

    private void OnProvokerRemoved(float amount)
    {
        ChangeSanity(amount, true);
    }

    private void OnProvokersChanged()
    {
        
    }

    private void Reset()
    {
        SetSanity(m_maxSanity);
        InvokeRepeating(nameof(Tick), m_initialTickDelay, m_decayRate);
    }

    private void Tick()
    {
        ChangeSanity(m_decayTickAmount, false);
    }

    private void ChangeSanity(float amount, bool add)
    {
        if (add)
        {
            m_currentSanity += amount;
        }
        else
        {
            m_currentSanity -= amount;
        }
        
        OnSanityChanged?.Invoke(m_currentSanity);
    }

    private void SetSanity(float amount)
    {
        m_currentSanity = amount;
        OnSanityChanged?.Invoke(m_currentSanity);
    }
}