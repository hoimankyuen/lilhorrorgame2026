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
        InteractableLure.OnLureInteracted += OnLureInteracted;
    }

    private void OnDisable()
    {
        Provoker.BroadcastingProvokerAdded -= OnProvokerAdded;
        Provoker.BroadcastingProvokerRemoved -= OnProvokerRemoved;
        Provoker.BroadcastingProvokerChanged -= OnProvokersChanged;
        InteractableLure.OnLureInteracted -= OnLureInteracted;
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

    private void OnLureInteracted(float amount)
    {
        ChangeSanity(amount, true);
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
            if (m_currentSanity + amount > m_maxSanity)
            {
                SetSanity(m_maxSanity);
                return;
            }

            m_currentSanity += amount;
        }
        else
        {
            if (m_currentSanity == 0)
            {
                return;
            }

            if (m_currentSanity - amount < 0)
            {
                SetSanity(0);
            }
            else
            {
                m_currentSanity -= amount;
            }
        }
        
        OnSanityChanged?.Invoke(m_currentSanity);

        if (m_currentSanity <= 0)
        {
            GameManager.Instance.LoseGame();
        }
    }

    private void SetSanity(float amount)
    {
        m_currentSanity = amount;
        OnSanityChanged?.Invoke(m_currentSanity);
    }
}