using System.Collections;
using MoonlightTools.GizmoTools;
using MoonlightTools.MathTools;
using UnityEngine;

public class MonsterSensitiveLight : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject m_dimLightModel;
    [SerializeField] private GameObject m_litLightModel;
    [SerializeField] private GameObject m_light;

    [Header("Settings")]
    [SerializeField] private RangedFloat m_checkInterval;
    [SerializeField] private float m_dimLightDistance;

    [Space]
    [SerializeField] private RangedFloat m_blinkDuration;
    [SerializeField] private RangedInt m_blinkCount;

    private bool m_prevWithinDistance;
    private float m_nextCheckTime;
    private Coroutine m_blinkCoroutine;
    
    // ======== Unity Events ========

    private void Start()
    {
        DimmingLight(true);
    }
    
    private void Update()
    {
        DimmingLight();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        GizmoUtils.DrawCircle(transform.position, m_dimLightDistance);
    }
    
    // ======== Functionalities ========
    
    private void DimmingLight(bool immediate = false)
    {
        if (Time.time < m_nextCheckTime && !immediate)
            return;
        m_nextCheckTime = Time.time + m_checkInterval.Random();

        bool withinDistance = false;
        foreach (Monster monster in Monster.Instances)
        {
            withinDistance |= Vector3.Distance(transform.position, monster.transform.position) < m_dimLightDistance;
        }
    
        if (m_prevWithinDistance == withinDistance && !immediate)
            return;
        m_prevWithinDistance = withinDistance;
        
        m_dimLightModel.SetActive(m_prevWithinDistance);
        m_litLightModel.SetActive(!m_prevWithinDistance);
        m_light.SetActive(!m_prevWithinDistance);

        if (!immediate)
        {
            if (m_blinkCoroutine != null)
                StopCoroutine(m_blinkCoroutine);

            m_blinkCoroutine = StartCoroutine(BlinkCoroutine(withinDistance, (int)m_blinkCount.Random()));
        }
    }

    private IEnumerator BlinkCoroutine(bool toDim, int count)
    {
        yield return new WaitForSeconds(m_blinkDuration.Random());
        for (int i = 0; i < count; i++)
        {
            m_dimLightModel.SetActive(!toDim);
            m_litLightModel.SetActive(toDim);
            m_light.SetActive(toDim);
            yield return new WaitForSeconds(m_blinkDuration.Random());
            m_dimLightModel.SetActive(toDim);
            m_litLightModel.SetActive(!toDim);
            m_light.SetActive(!toDim);            
            yield return new WaitForSeconds(m_blinkDuration.Random());
        }
    }
}
