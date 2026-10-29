using UnityEngine;
using UnityEngine.InputSystem;

public class DebugInteractableSceneMover : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera m_camera;

    [Header("Settings")]
    [SerializeField] private float m_moveSpeed;
    [SerializeField] private float m_rotateSpeed;
    
    void Update()
    {
        if (Keyboard.current.aKey.isPressed)
        {
            m_camera.transform.Translate(Vector3.left * (m_moveSpeed * Time.deltaTime));
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            m_camera.transform.Translate(Vector3.right * (m_moveSpeed * Time.deltaTime));
        }
        
        if (Keyboard.current.wKey.isPressed)
        {
            m_camera.transform.Translate(Vector3.forward * (m_moveSpeed * Time.deltaTime));
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            m_camera.transform.Translate(Vector3.back * (m_moveSpeed * Time.deltaTime));
        }

        if (Keyboard.current.qKey.isPressed)
        {
            m_camera.transform.Rotate(Vector3.down, m_rotateSpeed * Time.deltaTime);
        }
        else if (Keyboard.current.eKey.isPressed)
        {
            m_camera.transform.Rotate(Vector3.up, m_rotateSpeed * Time.deltaTime);

        }
    }
}
