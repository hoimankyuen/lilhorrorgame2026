using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WinCutscene : Cutscene
{
    [Header("References")]
    [SerializeField] private EyelidOverlay m_eyelidOverlay;
    [SerializeField] private Camera m_playerCamera;
    [SerializeField] private Monster m_monster;
    [SerializeField] private Transform m_exitCameraTransform;
    [SerializeField] private Light m_monsterLight;
    [SerializeField] private GameObject m_monsterVLight;
    [SerializeField] private Elevator m_elevator;
    [SerializeField] private Image m_cutsceneOverlay;
    
    [Space]
    [SerializeField] private EndScreen m_endScreen;
    
    protected override IEnumerator CutsceneSequence()
    {
        // cancels zoom and listen 
        m_playerCamera.DOFieldOfView(60f, 0.2f);
        DOTween.To(() => m_eyelidOverlay.CurrentEyelidProgress, x => m_eyelidOverlay.ShowEyelidProgress(x), 0f, 0.2f);
        yield return new WaitForSeconds(0.2f);
        
        // move monster
        m_monster.gameObject.SetActive(false);
        m_monsterLight.enabled = false;
        m_monsterVLight.SetActive(false);
        m_monster.transform.position = new Vector3(10.3f, 0f, -12.6f);
        m_monster.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        yield return null;
        m_monster.gameObject.SetActive(true);
        yield return null;
        m_monster.CancelNavigation();
        m_monster.CancelLookAt();
        yield return new WaitForSeconds(0.1f);

        // rotate back
        m_playerCamera.transform.DOMove(m_exitCameraTransform.transform.position, 2).SetEase(Ease.InOutQuad);
        m_playerCamera.transform.DORotate(m_exitCameraTransform.transform.eulerAngles, 2f).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(2f);
        
        // monster animation
        m_monsterLight.enabled = true;
        m_monsterVLight.SetActive(true);
        m_monster.SetHostileLevel(HostileLevel.Hostile);
        m_monster.Scream();
        m_playerCamera.transform.DOShakePosition(1f, Vector3.left * 0.1f);
        yield return new WaitForSeconds(0.5f);
        
        // close door
        m_elevator.CloseDoors();
        yield return new WaitForSeconds(1f);
        
        // elevator shake
        m_monsterLight.enabled = false;
        m_monsterVLight.SetActive(false);
        m_playerCamera.transform.DOShakePosition(7f, Vector3.up * 0.05f);
        yield return new WaitForSeconds(5f);
        
        // fade to white
        m_cutsceneOverlay.sprite = null;
        m_cutsceneOverlay.color = new Color(1f, 1f, 1f, 0f);
        m_cutsceneOverlay.DOColor(Color.white, 2f);
        yield return new WaitForSeconds(2f);
        yield return new WaitForSeconds(2f);
        
        // fade to black
        m_cutsceneOverlay.DOColor(Color.black, 2f);
        yield return new WaitForSeconds(2f);
        
        // show menu
        m_endScreen.Setup("You Escaped", "But, what next?");
        m_endScreen.Show();
    }
}