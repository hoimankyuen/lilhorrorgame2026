using System;
using MoonlightTools.GeneralTools;
using UnityEngine;

public class SanitySystem : MonoBehaviourSingleton<SanitySystem>
{
    public event Action<float> OnSanityChanged;
    
    [Header("References")]
    [SerializeField] private TimeManager m_timeManager;
    
    [Header("Settings")] 
    [SerializeField] private float m_maxSanity;
    [SerializeField] private float m_decayTickAmount;
    
    [Header("Stats")]
    [SerializeField] private float m_currentSanity;

    private float m_bonusTickAmount;
    
    private void Start()
    {
        Reset();
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameEnded += OnGameEnded;
        }

        if (m_timeManager != null)
        {
            m_timeManager.OnMinuteChange += Tick;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameEnded -= OnGameEnded;
        }
        if (m_timeManager != null)
        {
            m_timeManager.OnMinuteChange -= Tick;
        }
    }

    private void OnEnable()
    {
        InteractableLure.OnLureInteracted += OnLureInteracted;
        
    }

    private void OnDisable()
    {
        InteractableLure.OnLureInteracted -= OnLureInteracted;
 
    }

    private void OnLureInteracted(float amount)
    {
        ChangeSanity(amount, true);
    }

    private void OnGameEnded()
    {
        m_timeManager.OnMinuteChange -= Tick;
    }

    private void Reset()
    {
        SetSanity(m_maxSanity);
    }

    private void Tick()
    {
        ChangeSanity(m_decayTickAmount + m_bonusTickAmount, false);
    }

    public void ChangeSanity(float amount, bool add)
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
            GameManager.Instance.LoseGameBySanity();
        }
    }

    private void SetSanity(float amount)
    {
        m_currentSanity = amount;
        OnSanityChanged?.Invoke(m_currentSanity);
    }
    
    public void SetBonusTickAmount(float amount)
    {
        m_bonusTickAmount = amount;
    }
}