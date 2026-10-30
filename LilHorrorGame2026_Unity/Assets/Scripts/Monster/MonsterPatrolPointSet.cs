using MoonlightTools.GizmoTools;
using UnityEngine;

public class MonsterPatrolPointSet : MonoBehaviour
{
    [Header("GizmoSettings")]
    [SerializeField] private Color m_gizmoColor = Color.red;
    [SerializeField] private float m_gizmoSize = 0.5f;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = m_gizmoColor;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform patrolPoint = transform.GetChild(i);
            GizmoUtils.DrawCircle(patrolPoint.position, m_gizmoSize);
            Gizmos.DrawLine(patrolPoint.position, patrolPoint.position + patrolPoint.forward * m_gizmoSize * 2f);
        }
    }
    
    public Transform GetRandomPatrolPoint()
    {
        return transform.childCount > 0 ? transform.GetChild(Random.Range(0, transform.childCount)) : null;
    }
}
