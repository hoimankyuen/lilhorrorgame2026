using System;
using System.Collections;
using UnityEngine;

public abstract class Cutscene : MonoBehaviour
{
    public void PlayCutscene(Action onCompleted = null)
    {
        StartCoroutine(CutsceneSequenceBase(onCompleted));
    }

    private IEnumerator CutsceneSequenceBase(Action onCompleted)
    {
        yield return CutsceneSequence();
        onCompleted?.Invoke();
    }
    
    protected abstract IEnumerator CutsceneSequence();
}
