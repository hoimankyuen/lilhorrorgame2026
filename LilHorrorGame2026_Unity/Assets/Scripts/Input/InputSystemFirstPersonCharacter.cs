using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class InputSystemFirstPersonCharacter : MonoBehaviour
{
    private InputSystemFirstPersonControls inputActions;

    [Header("References")]
    [SerializeField] private CharacterController m_controller;
    [SerializeField] private Camera m_cam;
    
    [Header("Movement")]
    [SerializeField] private float m_movementSpeed = 2.0f;
    [SerializeField] private float m_lookSensitivity = 1.0f;
    
    [Header("Crouch")]
    [SerializeField] private float m_crouchHeight = 1.0f;
    [SerializeField] private float m_crouchSpeed = 6f;   // metres of height per second
    
    private float m_xRotation = 0f;
    
    [SerializeField] private float m_gravity = -9.81f;
    private Vector3 m_velocity;
    private bool m_grounded;
    
    [Header("Zoom")]
    [SerializeField] private float m_zoomFOV = 35.0f;
    [SerializeField] private float m_zoomSpeed = 9f;
    
    private float m_targetFOV;
    private float m_baseFOV;
    
    private float m_initHeight;
    private float m_initCenterY;
    private float m_initCamY;

    private void Awake()
    {
        inputActions = new InputSystemFirstPersonControls();
    }
    private void Start()
    {
        m_initHeight  = m_controller.height;
        m_initCenterY = m_controller.center.y;
        m_initCamY    = m_cam.transform.localPosition.y;
        Cursor.lockState = CursorLockMode.Locked;
        SetBaseFOV(m_cam.fieldOfView);
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void Update()
    {
        DoMovement();
        DoLooking();
        DoZoom();
        DoCrouch();
    }
    
    private int m_lookWarmupFrames = 3;

    private void DoLooking()
    {
        if (m_lookWarmupFrames > 0)
        {
            m_lookWarmupFrames--;
            return;
        }
        
        Vector2 looking = GetPlayerLook();
        float lookX = looking.x * m_lookSensitivity;
        float lookY = looking.y * m_lookSensitivity;

        m_xRotation -= lookY;
        m_xRotation = Mathf.Clamp(m_xRotation, -90f, 90f);
        
        m_cam.transform.localRotation = Quaternion.Euler(m_xRotation, 0f, 0f);
        
        transform.Rotate(Vector3.up * lookX);
    }

    private void DoMovement()
    {
        m_grounded = m_controller.isGrounded;
        if (m_grounded && m_velocity.y < 0)
        {
            m_velocity.y = -2f;
        }

        Vector2 movement = GetPlayerMovement();
        Vector3 move = transform.right * movement.x + transform.forward * movement.y;

        float totalMoveSpeed = m_movementSpeed;
        
        if (inputActions.FPSController.Run.ReadValue<float>() > 0 && Mathf.Approximately(m_controller.height, m_initHeight))
        {
            totalMoveSpeed = m_movementSpeed * 2;
        }

        m_controller.Move(move * (totalMoveSpeed * Time.deltaTime));

        m_velocity.y += m_gravity * Time.deltaTime;
        m_controller.Move(m_velocity * Time.deltaTime);
    }

    private void DoZoom()
    {
        m_targetFOV = inputActions.FPSController.Zoom.ReadValue<float>() > 0 ? m_zoomFOV : m_baseFOV;
        UpdateZoom();
    }
    
    private void DoCrouch()
    {
        bool wantToCrouch = inputActions.FPSController.Crouch.ReadValue<float>() > 0;
        float targetHeight = wantToCrouch || IsCeilingBlocked() ? m_crouchHeight : m_initHeight;

        float newHeight = Mathf.MoveTowards(m_controller.height, targetHeight, m_crouchSpeed * Time.deltaTime);
        float shrink = m_initHeight - newHeight;

        m_controller.height = newHeight;

        // Keep the bottom of the capsule at the feet
        Vector3 center = m_controller.center;
        center.y = m_initCenterY - shrink * 0.5f;
        m_controller.center = center;

        // Lower the camera by the same amount the top of the capsule dropped
        Vector3 camPos = m_cam.transform.localPosition;
        camPos.y = m_initCamY - shrink;
        m_cam.transform.localPosition = camPos;
    }
    
    private bool IsCeilingBlocked()
    {
        float heightToFullStand = m_initHeight - m_controller.height;
        if (heightToFullStand <= 0.001f) return false;

        float radius = m_controller.radius * 0.95f;
        Vector3 top = transform.TransformPoint(m_controller.center)
                      + Vector3.up * (m_controller.height * 0.5f - m_controller.radius);

        // Sphere sweep up through the space we'd grow into
        return Physics.SphereCast(top, radius, Vector3.up, out _, heightToFullStand + m_controller.skinWidth,
            ~0, QueryTriggerInteraction.Ignore);
    }

    private void UpdateZoom()
    {
        m_cam.fieldOfView = Mathf.Lerp(m_cam.fieldOfView, m_targetFOV, m_zoomSpeed * Time.deltaTime);
    }

    private void SetBaseFOV(float fov)
    {
        m_baseFOV = fov;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private Vector2 GetPlayerMovement()
    {
        return inputActions.FPSController.Move.ReadValue<Vector2>();
    }

    private Vector2 GetPlayerLook()
    {
        return inputActions.FPSController.Look.ReadValue<Vector2>();
    }
}