using UnityEngine;
using UnityEngine.InputSystem;

public class DebugProvokerActivator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera m_camera;
    
    // ======== Unity Events ========
    
    private void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.value);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMask.GetMask("Interactables")))
            {
                Provoker provoker = hit.collider.GetComponentInParent<Provoker>();
                if (provoker != null)
                {
                    provoker.Broadcast(true);
                }
            }
        }
    }
}
