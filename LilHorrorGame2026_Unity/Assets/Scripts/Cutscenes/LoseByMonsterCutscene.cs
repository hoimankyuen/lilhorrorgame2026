using System.Collections;
using DG.Tweening;
using MoonlightTools.GeneralTools;
using UnityEngine;
using UnityEngine.UI;

public class LoseByMonsterCutscene : Cutscene
{
    [Header("References")]
    [SerializeField] private EyelidOverlay m_eyelidOverlay;
    [SerializeField] private Camera m_playerCamera;
    [SerializeField] private Monster m_monster;
    [SerializeField] private Transform m_jumpScareCameraTransform;
    [SerializeField] private Image m_cutsceneOverlay;
    
    protected override IEnumerator CutsceneSequence()
    {
        // change monster to hostile
        m_monster.SetTVSpriteSet(Monster.TVSpriteSet.Eye);
        m_monster.SetHostileLevel(HostileLevel.Hostile);
        
        // turn towards monster, cancels zoom and listen
        Vector3 cameraStartPosition = m_playerCamera.transform.position;
        StartCoroutine(CoroutineUtils.LerpWithTime(0.6f, t =>
        {
            m_playerCamera.transform.position = Vector3.Lerp(cameraStartPosition, m_jumpScareCameraTransform.position, t);
        }));
        m_playerCamera.transform.DOMove(m_jumpScareCameraTransform.transform.position, 0.6f);
        m_playerCamera.transform.DORotate(m_jumpScareCameraTransform.transform.eulerAngles, 0.6f);
        m_playerCamera.DOFieldOfView(60f, 0.2f);
        DOTween.To(() => m_eyelidOverlay.CurrentEyelidProgress, x => m_eyelidOverlay.ShowEyelidProgress(x), 0f, 0.2f);
        yield return new WaitForSeconds(0.8f);
        
        // attack and die animation
        m_monster.Attack();
        yield return new WaitForSeconds(16f / 24f);
        m_cutsceneOverlay.color = Color.red;
        m_cutsceneOverlay.DOColor(new Color(1f, 0f, 0f, 0.5f), 0.5f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(1f);
        
        // fade to black
        m_cutsceneOverlay.DOColor(Color.black, 2f);
        yield return new WaitForSeconds(2f);

    }
}
