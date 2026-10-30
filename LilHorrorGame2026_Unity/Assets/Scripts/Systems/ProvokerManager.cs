using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProvokerManager : MonoBehaviour
{
    public static ProvokerManager Instance;
    
    [Header("References")]
    [SerializeField] private TimeManager m_timeManager;
    
    private static readonly List<Provoker> m_provokers = new();
    private readonly List<Provoker> m_activeProvokers = new();

    [Header("Settings")]
    [SerializeField] private int m_amountOfActiveProvokers = 10;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        m_timeManager.OnHourChange -= OnHourChange;
        m_timeManager.OnHourChange += OnHourChange;
    }

    private void OnEnable()
    {
        m_timeManager.OnHourChange += OnHourChange;
    }

    private void OnDisable()
    {
        m_timeManager.OnHourChange -= OnHourChange;
    }
    
    private void OnHourChange()
    {
        m_activeProvokers.Clear();
        
        for (int i = 0; i < m_amountOfActiveProvokers; i++)
        {
            Provoker randomProvoker = m_provokers[Random.Range(0, m_provokers.Count)]; 
            
            m_activeProvokers.Add(randomProvoker);
        }

        foreach (Provoker activeProvoker in m_activeProvokers)
        {
            if (activeProvoker.GetActiveState())
            {
                break;
            }
            activeProvoker.ChangeActiveState(true);
        }
    }

    public void RegisterProvoker(Provoker provoker)
    {
        m_provokers.Add(provoker);
    }
}