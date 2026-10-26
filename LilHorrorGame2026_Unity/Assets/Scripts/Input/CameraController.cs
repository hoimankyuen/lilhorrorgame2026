using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_mouse_sensitivity = 10f;
    
    public GameObject Player;
    private float m_xRotation;
    private float m_yRotation;
    private Vector2 m_mouseMovement;
    private float m_camera_y;
    private Vector3 m_pos;
    

    void Start()
    {
        m_camera_y=transform.position.y-Player.transform.position.y;
        Cursor.lockState=CursorLockMode.Locked;
    }
    
    void LateUpdate()
    {
        //Update Mouse position to player position
        m_pos=Player.transform.position;
        m_pos.y+=m_camera_y;
        transform.position=m_pos;
        //Get mouse movement in new input system
        m_mouseMovement = Mouse.current.delta.ReadValue();     
        m_xRotation -= m_mouseMovement.y * Time.deltaTime * m_mouse_sensitivity;   
        m_xRotation = Mathf.Clamp(m_xRotation,-90,90);
        m_yRotation += m_mouseMovement.x * Time.deltaTime * m_mouse_sensitivity;
        transform.rotation = Quaternion.Euler(m_xRotation, m_yRotation, 0);
        //Rotating the player
        Player.transform.localRotation = Quaternion.Euler(0, m_yRotation, 0);
    }
}
