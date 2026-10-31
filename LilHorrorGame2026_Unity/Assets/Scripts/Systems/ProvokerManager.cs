using System.Collections.Generic;
using MoonlightTools.GeneralTools;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProvokerManager : MonoBehaviourSingleton<ProvokerManager>
{
    [Header("References")]
    [SerializeField] private TimeManager m_timeManager;
    [SerializeField] private SanitySystem m_sanitySystem;
    
    [Header("Debug")]
    [SerializeField] private List<Provoker> m_registeredProvokers = new();
    [SerializeField] private List<Provoker> m_activeProvokers = new();

    [Header("Settings")]
    [SerializeField] private int m_amountOfActiveProvokersPerTick = 1;
    [SerializeField] private int m_maxActiveProvokers = 10;
    

    private void Start()
    {
        m_timeManager.OnMinuteChange += OnMinuteChange;
    }

    private new void OnDestroy()
    {
        m_timeManager.OnMinuteChange -= OnMinuteChange;
    }
    
    private void OnMinuteChange()
    {
        if (m_registeredProvokers.Count == 0)
        {
            return;
        }

        if (m_activeProvokers.Count >= m_maxActiveProvokers)
        {
            return;
        }

        if (m_timeManager.GetTimeOfDay().Minute == 15 || m_timeManager.GetTimeOfDay().Minute == 30 ||  m_timeManager.GetTimeOfDay().Minute == 45)
        {
            for (int i = 0; i < m_amountOfActiveProvokersPerTick; i++)
            {
                Provoker randomProvoker = m_registeredProvokers[Random.Range(0, m_registeredProvokers.Count)];

                if (!randomProvoker.GetActiveState())
                {
                    m_activeProvokers.Add(randomProvoker);
                }
            }

            foreach (Provoker activeProvoker in m_activeProvokers)
            {
                activeProvoker.ChangeActiveState(true);
            }
        }
        
        m_sanitySystem.SetBonusTickAmount(m_activeProvokers.Count * 0.1f);
    }
}