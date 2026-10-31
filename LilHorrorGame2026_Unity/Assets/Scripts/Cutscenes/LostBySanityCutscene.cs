using System.Collections;
using UnityEngine;

public class LostBySanityCutscene: Cutscene
{
    protected override IEnumerator CutsceneSequence()
    {
        yield return null;
    }
}
