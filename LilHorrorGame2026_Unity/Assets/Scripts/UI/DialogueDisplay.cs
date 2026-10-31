using System.Collections;
using DG.Tweening;
using MoonlightTools.GeneralTools;
using TMPro;
using UnityEngine;

public class DialogueDisplay : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup m_mainCanvasGroup;
    [SerializeField] private TMP_Text m_dialogueText;
    [SerializeField] private CanvasGroup m_canvasGroup;

    [Header("Settings")]
    [SerializeField] private float m_showDurationPerHundredCharacters;
    [SerializeField] private float m_defaultCloseDuration;
    [SerializeField] private float m_hideDuration;
    
    private Coroutine m_dialogueCoroutine;
    
    // ======== Unity Events ========
    
    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameEnded += OnGameEnded;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameEnded -= OnGameEnded;
        }
    }
    
    // ======== Game Manager Events ========
    
    private void OnGameEnded()
    {
        m_mainCanvasGroup.DOFade(0, 0.5f);
    }
    
    // ======== Appearance ========

    public void ShowDialogue(string dialogue)
    {
        m_dialogueCoroutine.StopIfRunning(this);
        m_dialogueCoroutine = StartCoroutine(DialogueSequence(dialogue, m_defaultCloseDuration));
    }
    
    public void ShowDialogue(string dialogue, float closedDuration)
    {
        m_dialogueCoroutine.StopIfRunning(this);
        m_dialogueCoroutine = StartCoroutine(DialogueSequence(dialogue, closedDuration));
    }

    private IEnumerator DialogueSequence(string dialogue,  float closedDuration)
    {
        yield return null;
        m_canvasGroup.alpha = 1f;
        float showDuration = dialogue.Length / 100f * m_showDurationPerHundredCharacters;
        float startTime = Time.time;
        while (Time.time - startTime < showDuration)
        {
            int insertIndex = (int)((Time.time - startTime) / showDuration * dialogue.Length);
            m_dialogueText.text = dialogue.Insert(insertIndex, "<alpha=#00>");
            yield return null;
        }
        m_dialogueText.text = dialogue;

        if (closedDuration > 0f)
        {
            yield return new WaitForSeconds(closedDuration);
            yield return CoroutineUtils.LerpWithTime(m_hideDuration, t => { m_canvasGroup.alpha = 1f - t; });
            m_dialogueText.text = "";
            yield return null;
            m_canvasGroup.alpha = 1f;
        }
        
        m_dialogueCoroutine = null;
    }
}
