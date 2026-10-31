using System.Collections;
using DG.Tweening;
using UnityEngine;

public class LostByMonsterCutscene : Cutscene
{
    [Header("References")]
    [SerializeField] private EyelidOverlay m_eyelidOverlay;
    [SerializeField] private Monster m_monster;
    [SerializeField] private Camera m_playerCamera;
    [SerializeField] private Transform m_jumpScareCameraTransform;
    [SerializeField] private CanvasGroup m_redOverlay;
    
    protected override IEnumerator CutsceneSequence()
    {
        m_monster.SetTVSpriteSet(Monster.TVSpriteSet.Eye);
        m_monster.SetHostileLevel(HostileLevel.Hostile);
        
        m_playerCamera.transform.DOMove(m_jumpScareCameraTransform.transform.position, 0.6f);
        m_playerCamera.transform.DORotate(m_jumpScareCameraTransform.transform.eulerAngles, 0.6f);
        m_playerCamera.DOFieldOfView(60f, 0.2f);
        DOTween.To(() => m_eyelidOverlay.CurrentEyelidProgress, x => m_eyelidOverlay.ShowEyelidProgress(x), 0f, 0.2f);
        yield return new WaitForSeconds(0.6f);
        yield return new WaitForSeconds(0.2f);
        
        m_monster.Attack();
        yield return new WaitForSeconds(16f / 24f);
        
        m_redOverlay.alpha = 1f;
        m_redOverlay.DOFade(0.5f, 1f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(1f);
    }
}
