using System.Collections;
using System.Collections.Generic;
using MoonlightTools.MathTools;
using UnityEngine;

public class ScreenStatic : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer m_staticSpriteRenderer;

    [Header("Settings")]
    [SerializeField] private int m_staticFPS = 24;
    [SerializeField] private int m_staticFrames = 4;
    
    [Space]
    [SerializeField] private ScreenStaticSpriteSet m_blankSpriteSet;
    [SerializeField] private ScreenStaticSpriteSet m_imageSpriteSet;
    [SerializeField] private RangedFloat m_toImageDuration;
    [SerializeField] private RangedFloat m_toBlankDuration;
    
    [Space]
    [SerializeField] private RangedFloat m_blinkInterval;
    [SerializeField] private RangedInt m_blinkCount;
    [SerializeField] private RangedFloat m_blinkDuration;
    
    private readonly List<int> m_spriteIndices = new();
    private int m_currentIndicesIndex;
    private float m_lastSpriteTime;

    private bool m_isBlinking;
    private float m_nextBlinkTime;
    
    private bool m_isBlank;
    private float m_nextSwapTime;
    
    // ======== Unity Events ========

    private void Awake()
    {
        SetupSpriteIndices();
    }

    private void Update()
    {
        Blink();
        SwapSpriteSet();
        ShowNextSprite();
    }

    private void OnEnable()
    {
        m_isBlinking = false;
        m_isBlank = false;
        
        GetNextSwapTime();
        GetNextBlinkTime();
    }
    
    // ======== Animation ========
    
    private void SetupSpriteIndices()
    {
        for (int i = 0; i < m_staticFrames; i++)
        {
            m_spriteIndices.Add(i);
        }
        ShuffleSpriteIndices();
    }
    
    private void ShuffleSpriteIndices()
    {
        for (int i = m_spriteIndices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (m_spriteIndices[i], m_spriteIndices[j]) = (m_spriteIndices[j], m_spriteIndices[i]);
        }
    }

    private void ShowNextSprite()
    {
        if (Time.time - m_lastSpriteTime < 1f / m_staticFPS)
            return;
            
        m_currentIndicesIndex++;
        if (m_currentIndicesIndex >= m_staticFrames)
        {
            m_currentIndicesIndex = 0;
            ShuffleSpriteIndices();
        }
        m_staticSpriteRenderer.sprite = GetCurrentSpriteSet().Get(m_currentIndicesIndex);
        m_lastSpriteTime = Time.time;
    }
    
    private ScreenStaticSpriteSet GetCurrentSpriteSet()
    {
        return m_isBlank || m_isBlinking ? m_blankSpriteSet : m_imageSpriteSet;
    }

    // ======== Blinking ========

    private void Blink()
    {
        if (Time.time < m_nextBlinkTime)
            return;
        
        StartCoroutine(BlinkSequence((int)m_blinkCount.Random()));
        GetNextBlinkTime();
    }

    private IEnumerator BlinkSequence(int count)
    {
        for (int i = 0; i < count; i++)
        {
            m_isBlinking = true;
            yield return new WaitForSeconds(m_blinkDuration.Random());
            m_isBlinking = false;
            yield return new WaitForSeconds(m_blinkDuration.Random());
        }
    }
    
    private void GetNextBlinkTime()
    {
        m_nextBlinkTime = Time.time + m_blinkInterval.Random();
    }
    
    // ======== Sprite Set ====
    
    public void SetBlankSpriteSet(ScreenStaticSpriteSet staticSpriteSet)
    {
        m_blankSpriteSet = staticSpriteSet;
        m_staticSpriteRenderer.sprite = GetCurrentSpriteSet().Get(m_currentIndicesIndex);
    }

    public void SetImageSpriteSet(ScreenStaticSpriteSet staticSpriteSet)
    {
        m_imageSpriteSet = staticSpriteSet;
        m_staticSpriteRenderer.sprite = GetCurrentSpriteSet().Get(m_currentIndicesIndex);
    }
    
    private void SwapSpriteSet()
    {
        if (Time.time < m_nextSwapTime)
            return;
        
        m_isBlank = !m_isBlank;
        GetNextSwapTime();
    }

    private void GetNextSwapTime()
    {
        m_nextSwapTime = Time.time + (m_isBlank ? m_toImageDuration.Random() : m_toBlankDuration.Random());
    }
}
