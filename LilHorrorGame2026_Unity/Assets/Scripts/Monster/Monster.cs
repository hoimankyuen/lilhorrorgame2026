using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public enum TVSpriteSet
    {
        Blank,
        Eye,
        Ear
    }
    
    public enum HostileLevel
    {
        Calm,
        Caution,
        Alert,
        Hostile
    }
    
    private static readonly int MovementSpeedAnimParam = Animator.StringToHash("MovementSpeed");
    private static readonly int HostilityAnimParam = Animator.StringToHash("Hostility");
    private static readonly int LookingAnimParam = Animator.StringToHash("Looking");
    private static readonly int ScreamingAnimParam = Animator.StringToHash("Screaming");

    [Header("Components")]
    [SerializeField] private NavMeshAgent m_agent;
    [SerializeField] private Transform m_TVTransform;
    [SerializeField] private Animator m_rigAnimator;
    [SerializeField] private Animator m_tvAnimator;
    [SerializeField] private SpriteRenderer m_tvSpriteRenderer;

    [Header("Facing Settings")]
    [SerializeField] private float m_facingSpeed;
    
    [Header("Movement Settings")]
    [SerializeField] private float m_walkSpeed;
    [SerializeField] private float m_runSpeed;
    [SerializeField] private float m_chaseSpeed;
    
    [Header("Detection Settings")] 
    [SerializeField] private float detectionAngle;
    [SerializeField] private float detectionRange;
    
    [Header("TV Display Settings")] 
    [SerializeField] private int m_tvFPS;
    [SerializeField] private List<Sprite> m_blankTVSprites;
    [SerializeField] private List<Sprite> m_eyeTVSprites;
    [SerializeField] private List<Sprite> m_earTVSprites;

    private bool m_isFacing;
    private bool m_facingCompleted;
    private Quaternion m_targetRotation;
    private float m_facingVelocity;
    
    private readonly List<int> m_tvSpriteIndices = new();
    private List<Sprite> m_CurrentTVSprites;
    private int m_currentTVSpriteIndicesIndex = 0;
    private float m_lastTVSpriteTime;

    // ======== Unity Events ========

    private void Awake()
    {
        SetupTVSpriteIndices();
        SetTVSpriteSet(TVSpriteSet.Eye);
        SetHostileLevel(HostileLevel.Calm);
    }
    
    private void Update()
    {
        UpdateFacing();
        UpdateMovementAnimation();
        ShowNextTVSprite();
    }

    private void OnDrawGizmos()
    {
        DrawDetectionGizmos();
    }
    
    
    // ======== Facing ========

    
    public void LookAt(Vector3 target)
    {
        m_isFacing = true;
        m_facingCompleted = false;
        target.y = transform.position.y;
        m_targetRotation = Quaternion.LookRotation(target - transform.position, Vector3.up); 
    }

    private void UpdateFacing()
    {
        if (!m_isFacing)
            return;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, m_targetRotation, m_facingSpeed * Time.deltaTime);
        if (Quaternion.Angle(transform.rotation, m_targetRotation) < 1f)
        {
            m_isFacing = false;
            m_facingCompleted = true;
        }
    }

    public void CancelLookAt()
    {
        m_isFacing = false;
        m_facingCompleted = false;
    }
    
    public bool HasLookedAt()
    {
        return m_facingCompleted;
    }
    
    // ======== Navigation ========

    
    public void WalkTo(Vector3 destination)
    {
        m_agent.speed = m_walkSpeed;
        m_agent.destination = destination;
    }

    public void RunTo(Vector3 destination)
    {
        m_agent.speed = m_runSpeed;
        m_agent.destination = destination;
    }
    
    public void ChaseTo(Vector3 destination)
    {
        m_agent.speed = m_chaseSpeed;
        m_agent.destination = destination;
    }

    public void CancelNavigation()
    {
        m_agent.destination = transform.position;
    }
    
    public bool HasArrivedToTarget()
    {
        return !m_agent.hasPath || Mathf.Approximately(m_agent.remainingDistance, 0f);
    }
    
    // ======== Movement Animation ========

    private void UpdateMovementAnimation()
    {
        m_facingVelocity = Mathf.Clamp01(m_facingVelocity + (m_isFacing ? 1f : -1f) * Time.deltaTime);
        m_rigAnimator.SetFloat(MovementSpeedAnimParam, Mathf.Max(m_facingVelocity * 2f, m_agent.velocity.magnitude));
    }
    
    // ======== Detection ========
    
    public bool CanDetect(Vector3 characterPosition, float characterHeight)
    {
        if (Vector3.Distance(transform.position, characterPosition) > detectionRange)
            return false;
        
        Vector3 viewForward = Vector3.ProjectOnPlane(m_TVTransform.forward, Vector3.up).normalized;
        if (Vector3.Angle(viewForward, characterPosition - transform.position) > detectionAngle)
            return false;
        
        Vector3 characterHeadPosition = characterPosition + Vector3.up * characterHeight - m_TVTransform.position;
        Vector3 sightLine = characterHeadPosition - m_TVTransform.position;
        if (Physics.Raycast(m_TVTransform.position, sightLine.normalized, sightLine.magnitude))
            return false;

        return true;
    }
    
    private void DrawDetectionGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 forward = Vector3.ProjectOnPlane(m_TVTransform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(m_TVTransform.right, Vector3.up).normalized;
        for (int i = -5; i < 5; i++)
        {
            float angle1 = detectionAngle / 10f * i * Mathf.Deg2Rad;
            float angle2 = detectionAngle / 10f * (i + 1) * Mathf.Deg2Rad;
            if (i == -5)
            {
                Gizmos.DrawLine(
                    transform.position,
                    transform.position + forward * detectionRange * Mathf.Cos(angle1) + right * detectionRange * Mathf.Sin(angle1));
            }
            Gizmos.DrawLine(
                transform.position + forward * detectionRange * Mathf.Cos(angle1) + right * detectionRange * Mathf.Sin(angle1),
                transform.position + forward * detectionRange * Mathf.Cos(angle2) + right * detectionRange * Mathf.Sin(angle2));
            if (i == 4)
            {
                Gizmos.DrawLine(
                    transform.position,
                    transform.position + forward * detectionRange * Mathf.Cos(angle2) + right * detectionRange * Mathf.Sin(angle2));
            }
        }
    }
    
    // ======== Special Action ========

    public void LookAround(bool look)
    {
        m_rigAnimator.SetBool(LookingAnimParam, look);
    }
    
    public void Scream()
    {
        m_rigAnimator.SetTrigger(ScreamingAnimParam);
    }
    
    // ======== TV ========

    private void SetupTVSpriteIndices()
    {
        for (int i = 0; i < 4; i++)
        {
            m_tvSpriteIndices.Add(i);
        }
        ShuffleTVSpriteIndices();
    }

    private void ShuffleTVSpriteIndices()
    {
        for (int i = m_tvSpriteIndices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (m_tvSpriteIndices[i], m_tvSpriteIndices[j]) = (m_tvSpriteIndices[j], m_tvSpriteIndices[i]);
        }
    }

    public void SetTVSpriteSet(TVSpriteSet spriteSet)
    {
        m_CurrentTVSprites = spriteSet switch
        {
            TVSpriteSet.Blank => m_blankTVSprites,
            TVSpriteSet.Eye => m_eyeTVSprites,
            TVSpriteSet.Ear => m_earTVSprites,
            _ => m_blankTVSprites
        };
        m_tvSpriteRenderer.sprite = m_CurrentTVSprites[m_currentTVSpriteIndicesIndex];
        m_lastTVSpriteTime = Time.time;
    }

    private void ShowNextTVSprite()
    {
        if (Time.time - m_lastTVSpriteTime < 1f / m_tvFPS)
            return;
            
        m_currentTVSpriteIndicesIndex++;
        if (m_currentTVSpriteIndicesIndex >= 4)
        {
            m_currentTVSpriteIndicesIndex = 0;
            ShuffleTVSpriteIndices();
        }
        m_tvSpriteRenderer.sprite = m_CurrentTVSprites[m_currentTVSpriteIndicesIndex];
        m_lastTVSpriteTime = Time.time;
    }

    public void SetHostileLevel(HostileLevel hostileLevel)
    {
        m_tvAnimator.SetInteger(HostilityAnimParam, (int)hostileLevel);
    }
}
