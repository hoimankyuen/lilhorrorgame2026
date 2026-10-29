using UnityEngine;
using UnityEngine.AI;

public class AgentRigidbodyPush : MonoBehaviour
{
    [SerializeField] private NavMeshAgent m_agent;
 
    [Tooltip("Force in newtons applied each physics step while overlapping.")]
    [SerializeField] float m_pushForce = 600f;
    
    [SerializeField] LayerMask m_pushLayers = ~0;
 
    [Tooltip("Below this speed the agent counts as stationary and doesn't push.")]
    [SerializeField] float m_minSpeed = 0.1f;
 
    void Reset()
    {
        m_agent = GetComponentInParent<NavMeshAgent>();
    }
 
    void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb == null || rb.isKinematic) return;
        if ((m_pushLayers.value & (1 << other.gameObject.layer)) == 0) return;
 
        Vector3 velocity = m_agent != null ? m_agent.velocity : Vector3.zero;
        velocity.y = 0f;
        if (velocity.sqrMagnitude < m_minSpeed * m_minSpeed) return;
        
        Vector3 contact = other.ClosestPoint(transform.position);
        
        rb.AddForceAtPosition(velocity.normalized * m_pushForce, contact, ForceMode.Force);
    }
}