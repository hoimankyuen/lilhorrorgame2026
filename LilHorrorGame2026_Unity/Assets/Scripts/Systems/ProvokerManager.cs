using System.Collections.Generic;
using MoonlightTools.GeneralTools;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProvokerManager : MonoBehaviourSingleton<ProvokerManager>
{
    [Header("References")] [SerializeField]
    private TimeManager m_timeManager;

    [SerializeField] private SanitySystem m_sanitySystem;

    [Header("Debug")] [SerializeField] private List<Provoker> m_registeredProvokersMainRoom = new();
    [SerializeField] private List<Provoker> m_registeredProvokersReception = new();
    [SerializeField] private List<Provoker> m_registeredProvokersBossOffice = new();
    [SerializeField] private List<Provoker> m_registeredProvokersConferenceRoom = new();
    [SerializeField] private List<Provoker> m_activeProvokers = new();

    [Header("Settings")] [SerializeField] private int m_amountOfActiveProvokersPerTick = 1;
    [SerializeField] private int m_maxActiveProvokers = 10;

    // Reused every tick to avoid allocating a new list
    private readonly List<Provoker> m_candidates = new();

    private void Start()
    {
        m_timeManager.OnMinuteChange += OnMinuteChange;
    }

    private new void OnDestroy()
    {
        m_timeManager.OnMinuteChange -= OnMinuteChange;
    }

    public void RemoveProvokerFromActive(Provoker provoker)
    {
        if (m_activeProvokers.Remove(provoker))
        {
            UpdateSanityBonus();
        }
    }

    private void OnMinuteChange()
    {
        int minute = m_timeManager.GetTimeOfDay().Minute;

        if (minute is 15 or 30 or 45 && m_activeProvokers.Count < m_maxActiveProvokers)
        {
            ActivateRandomProvokers();
        }

        UpdateSanityBonus();
    }

    private void ActivateRandomProvokers()
    {
        m_candidates.Clear();
        foreach (Provoker provoker in PickRandomRoom())
        {
            if (provoker != null && !provoker.GetActiveState())
            {
                m_candidates.Add(provoker);
            }
        }

        int activated = 0;

        while (activated < m_amountOfActiveProvokersPerTick
               && m_candidates.Count > 0
               && m_activeProvokers.Count < m_maxActiveProvokers)
        {
            // Draw a random candidate and remove it from the pool
            int index = Random.Range(0, m_candidates.Count);
            Provoker chosen = m_candidates[index];
            m_candidates[index] = m_candidates[^1];
            m_candidates.RemoveAt(m_candidates.Count - 1);

            if (m_activeProvokers.Contains(chosen))
            {
                continue; // already active, draw again
            }

            chosen.ChangeActiveState(true);
            m_activeProvokers.Add(chosen);
            activated++;
        }
    }

    private List<Provoker> PickRandomRoom() => Random.Range(0, 4) switch
    {
        0 => m_registeredProvokersMainRoom,
        1 => m_registeredProvokersReception,
        2 => m_registeredProvokersBossOffice,
        _ => m_registeredProvokersConferenceRoom,
    };

    private void UpdateSanityBonus()
    {
        m_sanitySystem.SetBonusTickAmount(m_activeProvokers.Count * 0.1f);
    }
}