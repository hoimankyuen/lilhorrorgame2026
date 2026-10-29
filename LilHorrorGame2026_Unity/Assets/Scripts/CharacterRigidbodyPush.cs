using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterRigidbodyPush : MonoBehaviour
{
    [Tooltip("Force in newtons applied while pushing.")]
    [SerializeField] float m_pushForce = 300f;
 
    [Tooltip("Only push objects on these layers (e.g. a 'Door' layer).")]
    [SerializeField] LayerMask m_pushLayers = ~0;
 
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb == null || rb.isKinematic) return;
        if ((m_pushLayers.value & (1 << hit.gameObject.layer)) == 0) return;
        
        if (hit.moveDirection.y < -0.3f) return;
        
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z).normalized;
        
        rb.AddForceAtPosition(pushDir * m_pushForce * Time.deltaTime, hit.point, ForceMode.Impulse);
    }
}
