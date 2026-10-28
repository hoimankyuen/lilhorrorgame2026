using System;
using UnityEngine;

public class SanitySystem : MonoBehaviour
{
    public static SanitySystem Instance { get; private set; }
    
    [Header("Settings")] 
    [SerializeField] private float m_maxSanity;
    [SerializeField] private float m_decayRate;
    [SerializeField] private float m_decayTickAmount;
    [SerializeField] private float m_initialTickDelay = 5f;
    
    [Header("Stats")]
    [SerializeField] private float m_currentSanity;
    
    private void Start()
    {
        SingletonSetup();
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
        m_currentSanity -= amount;
    }

    private void OnProvokerRemoved(float amount)
    {
        m_currentSanity += amount;
    }

    private void OnProvokersChanged()
    {
        
    }

    private void SingletonSetup()
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

    private void Reset()
    {
        m_currentSanity = m_maxSanity;
        InvokeRepeating(nameof(Tick), m_initialTickDelay, m_decayRate);
    }

    private void Tick()
    {
        m_currentSanity -= m_decayTickAmount;
    }
}