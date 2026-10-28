using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public enum TVSpriteSet
    {
        Blank,
        Eye,
        Ear
    }
    
    private static readonly int MovementSpeedAnimParam = Animator.StringToHash("MovementSpeed");

    [Header("Components")]
    [SerializeField] private NavMeshAgent m_agent;
    [SerializeField] private Animator m_rigAnimator;
    [SerializeField] private Animator m_tvAnimator;
    [SerializeField] private SpriteRenderer m_tvSpriteRenderer;

    [Header("Settings")]
    [SerializeField] private float m_walkSpeed;
    [SerializeField] private float m_runSpeed;

    [Space] 
    [SerializeField] private int m_tvFPS;
    [SerializeField] private List<Sprite> m_blankTVSprites;
    [SerializeField] private List<Sprite> m_eyeTVSprites;
    [SerializeField] private List<Sprite> m_earTVSprites;
    
    private readonly List<int> m_tvSpriteIndices = new();
    private List<Sprite> m_CurrentTVSprites;
    private int m_currentTVSpriteIndicesIndex = 0;
    private float m_lastTVSpriteTime;

    // ======== Unity Events ========

    private void Awake()
    {
        SetupTVSpriteIndices();
        SetTVSpriteSet(TVSpriteSet.Eye);
    }
    
    private void Update()
    {
        UpdateAnimation();
        ShowNextTVSprite();
    }
    
    // ======== Navigation ========

    public void WalkTo(Vector3 destination)
    {
        m_agent.speed = m_walkSpeed;
        m_agent.destination = destination;
    }

    public void RunTo(Vector3 destination)
    {
        m_agent.speed = m_runSpeed;
        m_agent.destination = destination;
    }

    private void UpdateAnimation()
    {
        m_rigAnimator.SetFloat(MovementSpeedAnimParam, m_agent.velocity.magnitude);
    }
    
    // ======== TV ========

    private void SetupTVSpriteIndices()
    {
        for (int i = 0; i < 4; i++)
        {
            m_tvSpriteIndices.Add(i);
        }
        ShuffleTVSpriteIndices();
    }

    private void ShuffleTVSpriteIndices()
    {
        for (int i = m_tvSpriteIndices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (m_tvSpriteIndices[i], m_tvSpriteIndices[j]) = (m_tvSpriteIndices[j], m_tvSpriteIndices[i]);
        }
    }

    private void SetTVSpriteSet(TVSpriteSet spriteSet)
    {
        m_CurrentTVSprites = spriteSet switch
        {
            TVSpriteSet.Blank => m_blankTVSprites,
            TVSpriteSet.Eye => m_eyeTVSprites,
            TVSpriteSet.Ear => m_earTVSprites,
            _ => m_blankTVSprites
        };
        m_tvSpriteRenderer.sprite = m_CurrentTVSprites[m_currentTVSpriteIndicesIndex];
        m_lastTVSpriteTime = Time.time;
    }

    private void ShowNextTVSprite()
    {
        if (Time.time - m_lastTVSpriteTime < 1f / m_tvFPS)
            return;
            
        m_currentTVSpriteIndicesIndex++;
        if (m_currentTVSpriteIndicesIndex >= 4)
        {
            m_currentTVSpriteIndicesIndex = 0;
            ShuffleTVSpriteIndices();
        }
        m_tvSpriteRenderer.sprite = m_CurrentTVSprites[m_currentTVSpriteIndicesIndex];
        m_lastTVSpriteTime = Time.time;
    }
}
