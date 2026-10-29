using System.Collections.Generic;
using DG.Tweening;
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
    private static readonly int SoftLookingAnimParam = Animator.StringToHash("SoftLooking");
    private static readonly int HardLookingAnimParam = Animator.StringToHash("HardLooking");
    private static readonly int ScreamingAnimParam = Animator.StringToHash("Screaming");

    [Header("Components")]
    [SerializeField] private NavMeshAgent m_agent;
    [SerializeField] private Transform m_TVTransform;
    [SerializeField] private Animator m_rigAnimator;
    [SerializeField] private Animator m_tvAnimator;
    [SerializeField] private SpriteRenderer m_tvSpriteRenderer;
    [SerializeField] private Transform m_tvLightTransform;
    [SerializeField] private Light m_tvLight;

    [Header("Facing Settings")]
    [SerializeField] private float m_facingSpeed;
    
    [Header("Movement Settings")]
    [SerializeField] private float m_walkSpeed;
    [SerializeField] private float m_runSpeed;
    [SerializeField] private float m_chaseSpeed;
    
    [Header("Detection Settings")] 
    [SerializeField] private float forwardDetectionAngle;
    [SerializeField] private float forwardDetectionRange;
    [SerializeField] private float peripheralDetectionAngle;
    [SerializeField] private float peripheralDetectionRange;
    
    [Header("TV Display Settings")] 
    [SerializeField] private int m_tvFPS;
    [SerializeField] private List<Sprite> m_blankTVSprites;
    [SerializeField] private List<Sprite> m_eyeTVSprites;
    [SerializeField] private List<Sprite> m_earTVSprites;
    [SerializeField] private Color m_calmLightColour;
    [SerializeField] private Color m_cautionLightColour;
    [SerializeField] private Color m_alertLightColour;
    [SerializeField] private Color m_hostileLightColour;
    
    private bool m_isFacing;
    private bool m_facingCompleted;
    private Quaternion m_targetRotation;
    private float m_facingVelocity;

    private Lure m_nearestBroadcastingLure;
    
    private readonly List<int> m_tvSpriteIndices = new();
    private List<Sprite> m_CurrentTVSprites;
    private int m_currentTVSpriteIndicesIndex = 0;
    private float m_lastTVSpriteTime;
    private readonly Queue<Quaternion> m_tvLightRotations = new();

    // ======== Unity Events ========

    private void Awake()
    {
        SetupTVSpriteIndices();
        SetTVSpriteSet(TVSpriteSet.Eye);
        SetHostileLevel(HostileLevel.Calm);

        Lure.BroadcastingLuresChanged += OnBroadcastingLuresChanged;
    }

    private void OnDestroy()
    {
        Lure.BroadcastingLuresChanged -= OnBroadcastingLuresChanged;

    }
    
    private void Update()
    {
        UpdateFacing();
        UpdateMovementAnimation();
        ShowNextTVSprite();
        StabilizeTVLight();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        DrawDetectionGizmos(forwardDetectionAngle, forwardDetectionRange);
        DrawDetectionGizmos(peripheralDetectionAngle, peripheralDetectionRange);
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
    
    public bool WithinForwardSight(Vector3 targetPosition, float targetHeight)
    {
        return WithinSight(targetPosition, targetHeight, forwardDetectionAngle, forwardDetectionRange);
    }

    public bool WithinPeripheralSight(Vector3 targetPosition, float targetHeight)
    {
        return WithinSight(targetPosition, targetHeight, peripheralDetectionAngle, peripheralDetectionRange);
    }

    private bool WithinSight(Vector3 targetPosition, float targetHeight, float angle, float range)
    {
        if (Vector3.Distance(transform.position, targetPosition) > range)
            return false;
        
        Vector3 viewForward = Vector3.ProjectOnPlane(m_TVTransform.forward, Vector3.up).normalized;
        if (Vector3.Angle(viewForward, targetPosition - transform.position) > angle)
            return false;
        
        Vector3 characterHeadPosition = targetPosition + Vector3.up * targetHeight;
        Vector3 sightLine = characterHeadPosition - m_TVTransform.position;
        if (Physics.Raycast(m_TVTransform.position, sightLine.normalized, out RaycastHit hit, sightLine.magnitude))
        {
            return hit.collider.gameObject.layer == LayerMask.NameToLayer("Player");
        }
        return true;
    }
    
    private void DrawDetectionGizmos(float angle, float range)
    {
        Vector3 forward = Vector3.ProjectOnPlane(m_TVTransform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(m_TVTransform.right, Vector3.up).normalized;
        for (int i = -5; i < 5; i++)
        {
            float angle1 = angle / 10f * i * Mathf.Deg2Rad;
            float angle2 = angle / 10f * (i + 1) * Mathf.Deg2Rad;
            if (i == -5)
            {
                Gizmos.DrawLine(
                    transform.position,
                    transform.position + forward * range * Mathf.Cos(angle1) + right * range * Mathf.Sin(angle1));
            }
            Gizmos.DrawLine(
                transform.position + forward * range * Mathf.Cos(angle1) + right * range * Mathf.Sin(angle1),
                transform.position + forward * range * Mathf.Cos(angle2) + right * range * Mathf.Sin(angle2));
            if (i == 4)
            {
                Gizmos.DrawLine(
                    transform.position,
                    transform.position + forward * range * Mathf.Cos(angle2) + right * range * Mathf.Sin(angle2));
            }
        }
    }
    
    // ======== Listening ========

    private void OnBroadcastingLuresChanged()
    {
        m_nearestBroadcastingLure = Lure.GetNearestBroadcastingLure(transform.position);
    }
    
    public bool CanHearLure(out Vector3 nearestLurePosition)
    {
        nearestLurePosition = m_nearestBroadcastingLure != null ? m_nearestBroadcastingLure.transform.position : Vector3.zero;
        return m_nearestBroadcastingLure != null;
    }
    
    // ======== Special Action ========

    public void SoftLookAround(bool look)
    {
        m_rigAnimator.SetBool(SoftLookingAnimParam, look);
    }
    
    public void HardLookAround(bool look)
    {
        m_rigAnimator.SetBool(HardLookingAnimParam, look);
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
        m_tvLight.DOColor(hostileLevel switch
        {
            HostileLevel.Calm => m_calmLightColour,
            HostileLevel.Caution => m_cautionLightColour,
            HostileLevel.Alert => m_alertLightColour,
            HostileLevel.Hostile => m_hostileLightColour,
            _ => m_calmLightColour
        }, 0.25f);
    }

    private void StabilizeTVLight()
    {
        if (m_tvLightRotations.Count > 0)
        {
            Quaternion average = m_tvLightTransform.rotation;
            int i = 0;
            foreach (Quaternion rotation in m_tvLightRotations)
            {
                i++;
                float weight = 1.0f / i;
                average = Quaternion.Slerp(average, rotation, weight);
            }
            m_tvLight.transform.rotation = average;
        }
        
        m_tvLightRotations.Enqueue(m_tvLightTransform.rotation);
        if (m_tvLightRotations.Count > 20)
        {
            m_tvLightRotations.Dequeue();
        }
    }
}
