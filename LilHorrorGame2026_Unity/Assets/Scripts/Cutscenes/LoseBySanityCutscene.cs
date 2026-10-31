using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoonlightTools.GeneralTools;
using MoonlightTools.MathTools;
using UnityEngine;
using UnityEngine.UI;

public class LoseBySanityCutscene: Cutscene
{
    [Header("References")]
    [SerializeField] private EyelidOverlay m_eyelidOverlay;
    [SerializeField] private Camera m_playerCamera;
    [SerializeField] private Monster m_monster;
    [SerializeField] private Transform m_possessCameraTransform;
    [SerializeField] private Image m_cutsceneOverlay;

    [Space]
    [SerializeField] private EndScreen m_endScreen;
    
    [Space]
    [SerializeField] private List<Sprite> m_staticSprites;
    
    protected override IEnumerator CutsceneSequence()
    {
        // cancels zoom and listen 
        m_playerCamera.DOFieldOfView(60f, 0.2f);
        DOTween.To(() => m_eyelidOverlay.CurrentEyelidProgress, x => m_eyelidOverlay.ShowEyelidProgress(x), 0f, 0.2f);
        yield return new WaitForSeconds(0.2f);
        
        // static overlay
        List<int> staticIndices = new List<int>();
        for (int i = 0; i < m_staticSprites.Count; i++)
        {
            staticIndices.Add(i);
        }
        ShuffleSpriteIndices(staticIndices);
        int index = 0;
        m_cutsceneOverlay.color = new Color(1f, 1f, 1f, 0f);
        m_cutsceneOverlay.sprite = m_staticSprites[index];
        StartCoroutine(CoroutineUtils.LerpWithTime(2f, t =>
        {
            index++;
            if (index >= m_staticSprites.Count)
            {
                ShuffleSpriteIndices(staticIndices);
                index = 0;
            }

            m_cutsceneOverlay.sprite = m_staticSprites[staticIndices[index]];
        }));
        yield return CoroutineUtils.LerpWithTime(1.5f, t =>
        {
            m_cutsceneOverlay.color = new Color(1f, 1f, 1f, t);
        });
        yield return new WaitForSeconds(0.1f);
        
        // blink in
        yield return CoroutineUtils.LerpWithTime(0.4f, t =>
        {
            m_eyelidOverlay.ShowEyelidProgress(t);
        });
        
        // reposition player, camera and monster
        m_playerCamera.transform.position = new Vector3(-1000, 0, -1000);
        m_cutsceneOverlay.color = new Color(1f, 1f, 1f, 0f);
        m_monster.gameObject.SetActive(false);
        m_monster.transform.position = new Vector3(-5f, 0f, -1f);
        m_monster.transform.rotation = Quaternion.identity;
        m_playerCamera.transform.position = m_possessCameraTransform.position;
        m_playerCamera.transform.rotation = m_possessCameraTransform.rotation;
        yield return new WaitForSeconds(1.5f);
        
        // blink out
        yield return CoroutineUtils.LerpWithTime(0.4f, t =>
        {
            m_eyelidOverlay.ShowEyelidProgress(1 - t);
        });
        
        // look around
        yield return new WaitForSeconds(2f);
        m_playerCamera.transform.DORotate(new Vector3(0f, 30f, 0f), 1f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(1f);
        m_playerCamera.transform.DORotate(new Vector3(0f, -30f, 0f), 2f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(2f);
        m_playerCamera.transform.DORotate(new Vector3(0f, 0f, 0f), 1f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(1f);
        
        // zoom out camera
        m_monster.gameObject.SetActive(true);
        yield return null;
        m_monster.CancelNavigation();
        m_monster.CancelLookAt();
        m_playerCamera.transform.DOMove(m_possessCameraTransform.position - m_possessCameraTransform.forward * 5f, 10f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(8f);

        // fade to black
        m_cutsceneOverlay.sprite = null;
        m_cutsceneOverlay.color = new Color(0f, 0f, 0f, 0f);
        m_cutsceneOverlay.DOColor(Color.black, 2f);
        yield return new WaitForSeconds(2f);
        
        // show menu
        m_endScreen.Setup("You Transformed", "Lived long enough to be the villain eh?");
        m_endScreen.Show();
    }
    
    private void ShuffleSpriteIndices(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
    
    
}
