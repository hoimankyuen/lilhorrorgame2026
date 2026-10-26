using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_movement_force = 50000;
    
    [Header("References")]
    [SerializeField] private Rigidbody m_rb;
    
    private FPS_Controls m_input_control;
    private Vector2 m_move;
    
    void Start()
    {
        m_input_control = new FPS_Controls();
        m_input_control.Player.Enable();
        m_move = Vector2.zero;
    }
    
    private void FixedUpdate()
    {
        Move();
    }
    
    private void Move()
    {
        m_move = m_input_control.Player.Movement.ReadValue<Vector2>();
        float forcez = m_move.x*m_movement_force * Time.deltaTime;    
        float forcex = m_move.y*m_movement_force * Time.deltaTime;    
        m_rb.AddForce(transform.forward * forcex, ForceMode.Force);
        m_rb.AddForce(transform.right * forcez, ForceMode.Force);

    }
}