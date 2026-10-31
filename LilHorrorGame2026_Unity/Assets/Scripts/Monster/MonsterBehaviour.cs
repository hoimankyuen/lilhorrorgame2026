using System.Text.RegularExpressions;
using MoonlightTools.AttributeUtils;
using MoonlightTools.MathTools;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;

[RequireComponent(typeof(Monster))]
public class MonsterBehaviour : MonoBehaviour
{
    private class BehaviourState
    {
        protected MonsterBehaviour Behaviour { get; private set; }
        protected Monster Monster => Behaviour.m_monster;

        protected BehaviourState(MonsterBehaviour behaviour)
        {
            Behaviour = behaviour;
        }
        
        public virtual void OnStateEnter() { }
        public virtual void OnStateUpdate() { }
        public virtual void OnStateExit() { }

        protected void ChangeState(BehaviourState newState)
        {
            Behaviour.ChangeState(newState);
        }

        public override string ToString()
        {
            return Regex.Replace(base.ToString().Replace("MonsterBehaviour+", ""), "(\\B[A-Z])", " $1");
        }
    }

    [Header("Status")] 
    [SerializeField, Locked] private string m_currentStateName = "None";

    [Header("Shared Settings")]
    [SerializeField] private Transform m_playerBodyTransform; // to be changed
    [SerializeField] private Transform m_playerHeadTransform; // to be changed
    
    [Header("Idle State Settings")]
    [SerializeField] private RangedFloat m_idleDuration;
    [SerializeField] private RangedFloat m_idleExtensionDuration;
    [SerializeField] private float m_turnChance;
    [SerializeField] private float m_wanderChance;
    [SerializeField] private float m_lookChance;
    
    [Header("Turn State Settings")]
    [SerializeField] private float m_minTurnAngle;
    
    [Header("Wander State Settings")]
    [SerializeField] private MonsterPatrolPointSet m_wanderPatrolPointSet;
    
        
    [Header("Investigate Completed State Settings")]
    [SerializeField] private float m_investigateCompletedDuration;
    
    [Header("Look State Settings")]
    [SerializeField] private float m_lookDuration;
    
    [Header("Scream State Settings")]
    [SerializeField] private float m_screamDuration;
    
    [Header("Suspect State Settings")]
    [SerializeField] private float m_suspectDuration;
    [SerializeField] private float m_easeDuration;
    
    [Header("Target Lost State Settings")]
    [SerializeField] private float m_targetLostDuration;

    private Monster m_monster;

    private bool m_stateMachineRunning;
    private BehaviourState m_currentState;
    private BehaviourState m_nextState;
    private bool m_changingState;
    
    // ======== Unity Events ========

    private void Awake()
    {
        m_monster = GetComponent<Monster>();
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameEnded += OnGameEnded;
        }
        
        StartStateMachine(new IdleBehaviourState(this));
    }
    
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameEnded -= OnGameEnded;
        }
    }
    
    private void Update()
    {
        StateMachine();
    }
    
    // ======== State Machine ========

    private void StateMachine()
    {
        m_currentState?.OnStateUpdate();

        if (m_changingState)
        {
            m_currentState?.OnStateExit();
            m_currentState = m_nextState;
            m_currentState?.OnStateEnter();
            m_nextState = null;
            m_changingState = false;

            m_currentStateName = m_currentState != null ? m_currentState.ToString() : "None";
        }
    }

    private void StartStateMachine(BehaviourState nextState)
    {
        m_stateMachineRunning = true;
        ChangeState(nextState);
    }

    private void StopStateMachine()
    {
        m_stateMachineRunning = false;
        m_currentState = null;
        m_nextState = null;
    }
    
    private void ChangeState(BehaviourState nextState)
    {
        if (!m_stateMachineRunning)
            return;
        
        m_nextState = nextState;
        m_changingState = true;
    }

    private void OnGameEnded()
    {
        StopStateMachine();
        m_monster.CancelLookAt();
        m_monster.CancelNavigation();
        m_monster.ResetAnimator();
    }
    
    // ======== Shared Utilities ========
    
    private Vector3 PlayerPosition => m_playerBodyTransform.position;
    private float PlayerHeight => m_playerHeadTransform.position.y - m_playerBodyTransform.position.y;

    private static int GetFromWeightedChances(float[] chances)
    {
        float totalChance = 0f;
        for (int i = 0; i < chances.Length; i++)
        {
            totalChance += chances[i];
        }
        float random = Random.Range(0f, totalChance);
        for (int i = 0; i < chances.Length; i++)
        {
            if (random <= chances[i])
            {
                return i;
            }
            random -= chances[i];
        }
        return chances.Length - 1;
    }
    
    // ======== Template State ========
    
    private class TemplateBehaviourState : BehaviourState
    {
        public TemplateBehaviourState(MonsterBehaviour behaviour) : base(behaviour) {}

        public override void OnStateEnter()
        {
        }

        public override void OnStateUpdate()
        {
        }

        public override void OnStateExit()
        {
        }
    }
    
    // ======== Idle State ========
    
    private class IdleBehaviourState : BehaviourState
    {
        private readonly bool m_isExtension;
        private float m_nextActionTime;
        private bool m_facingWall;

        public IdleBehaviourState(MonsterBehaviour behaviour, bool isExtension = false) : base(behaviour)
        {
            m_isExtension = isExtension;
        }
        
        public override void OnStateEnter()
        {
            m_nextActionTime = Time.time + (m_isExtension ? Behaviour.m_idleDuration.Random() : Behaviour.m_idleExtensionDuration.Random());
            m_facingWall = Monster.IsFacingWall();
        }

        public override void OnStateUpdate()
        {
            if (Monster.WithinForwardSight(Behaviour.PlayerPosition, Behaviour.PlayerHeight))
            {
                ChangeState(new SuspectBehaviourState(Behaviour));
            }
            else if (Monster.CanHearLure(out Vector3 nearestLurePosition))
            {
                ChangeState(new InvestigateBehaviourState(Behaviour, nearestLurePosition));
            }
            else if (m_facingWall)
            {
                ChangeState(new TurnBehaviourState(Behaviour));
            }
            else if (Time.time > m_nextActionTime)
            {
                float[] chances =
                {
                    Behaviour.m_turnChance,
                    Behaviour.m_wanderChance,
                    Behaviour.m_lookChance
                };
                switch (GetFromWeightedChances(chances))
                {
                    case 0:
                        ChangeState(new TurnBehaviourState(Behaviour));
                        break;
                    case 1:
                        ChangeState(new WanderBehaviourState(Behaviour));
                        break;
                    case 2:
                        ChangeState(new LookBehaviourState(Behaviour));
                        break;
                    default:
                        ChangeState(new LookBehaviourState(Behaviour));
                        break;
                }
            }
        }

        public override void OnStateExit()
        {
        }
    }
    
    // ======== Turn State ========
    
    private class TurnBehaviourState : BehaviourState
    {
        private float m_nextActionTime;
        
        public TurnBehaviourState(MonsterBehaviour behaviour) : base(behaviour) {}

        public override void OnStateEnter()
        {
            Quaternion rotation = Quaternion.AngleAxis(Random.Range(Behaviour.m_minTurnAngle, 180f) * (Random.Range(0, 2) * 2 - 1), Vector3.up);
            Vector3 newForward = rotation * Monster.transform.forward;
            Monster.LookAt(Monster.transform.position + newForward);
        }

        public override void OnStateUpdate()
        {
            if (Monster.WithinForwardSight(Behaviour.PlayerPosition, Behaviour.PlayerHeight))
            {
                ChangeState(new SuspectBehaviourState(Behaviour));
            }
            else if (Monster.CanHearLure(out Vector3 nearestLurePosition))
            {
                ChangeState(new InvestigateBehaviourState(Behaviour, nearestLurePosition));
            }
            else if (Monster.HasLookedAt())
            {
                ChangeState(new IdleBehaviourState(Behaviour)); 
            }
        }

        public override void OnStateExit()
        {
            Monster.CancelLookAt();
        }
    }
    
    // ======== Wander State ========
    
    private class WanderBehaviourState : BehaviourState
    {
        private float m_nextActionTime;
        
        public WanderBehaviourState(MonsterBehaviour behaviour) : base(behaviour) {}

        public override void OnStateEnter()
        {
            Transform patrolPoint = Behaviour.m_wanderPatrolPointSet.GetRandomPatrolPoint();
            if (patrolPoint != null)
            {
                Monster.WalkTo(patrolPoint.position);
            }
        }

        public override void OnStateUpdate()
        {
            if (Monster.WithinForwardSight(Behaviour.PlayerPosition, Behaviour.PlayerHeight))
            {
                ChangeState(new SuspectBehaviourState(Behaviour));
            }
            else if (Monster.CanHearLure(out Vector3 nearestLurePosition))
            {
                ChangeState(new InvestigateBehaviourState(Behaviour, nearestLurePosition));
            }
            else if (Monster.HasArrivedToTarget())
            {
                ChangeState(new IdleBehaviourState(Behaviour)); 
            }
        }

        public override void OnStateExit()
        {
            Monster.CancelNavigation();
        }
    }
    
    // ======== Look State ========
    
    private class LookBehaviourState : BehaviourState
    {
        private float m_nextActionTime;
        
        public LookBehaviourState(MonsterBehaviour behaviour) : base(behaviour) {}

        public override void OnStateEnter()
        {
            m_nextActionTime = Time.time + Behaviour.m_lookDuration;
            Monster.SoftLookAround(true);
        }

        public override void OnStateUpdate()
        {

            if (Monster.WithinForwardSight(Behaviour.PlayerPosition, Behaviour.PlayerHeight))
            {
                ChangeState(new SuspectBehaviourState(Behaviour));
            }
            else if (Monster.CanHearLure(out Vector3 nearestLurePosition))
            {
                ChangeState(new InvestigateBehaviourState(Behaviour, nearestLurePosition));
            }
            else if (Time.time > m_nextActionTime)
            {
                ChangeState(new IdleBehaviourState(Behaviour, true)); 
            }
        }

        public override void OnStateExit()
        {
            Monster.SoftLookAround(false);
        }
    }
    
    // ======== Investigate State ========
    
    private class InvestigateBehaviourState : BehaviourState
    {
        private readonly Vector3 m_targetPosition;

        public InvestigateBehaviourState(MonsterBehaviour behaviour, Vector3 targetPosition) : base(behaviour)
        {
            m_targetPosition = targetPosition;
        }

        public override void OnStateEnter()
        {
            Monster.RunTo(m_targetPosition);
            Monster.SetHostileLevel(HostileLevel.Caution);
        }

        public override void OnStateUpdate()
        {
            if (Monster.WithinForwardSight(Behaviour.PlayerPosition, Behaviour.PlayerHeight))
            {
                ChangeState(new SuspectBehaviourState(Behaviour, true));
            }
            else if (Monster.HasArrivedToTarget())
            {
                ChangeState(new InvestigateCompletedBehaviourState(Behaviour)); 
            }
        }

        public override void OnStateExit()
        {
            Monster.CancelNavigation();
        }
    }

    // ======== Investigation Completed State ========
    
    private class InvestigateCompletedBehaviourState : BehaviourState
    {
        private float m_nextActionTime;
        
        public InvestigateCompletedBehaviourState(MonsterBehaviour behaviour) : base(behaviour) {}

        public override void OnStateEnter()
        {
            m_nextActionTime = Time.time + Behaviour.m_investigateCompletedDuration;
            Monster.HardLookAround(true);
        }

        public override void OnStateUpdate()
        {
            if (Monster.WithinForwardSight(Behaviour.PlayerPosition, Behaviour.PlayerHeight))
            {
                ChangeState(new SuspectBehaviourState(Behaviour, true));
            }
            else if (Time.time > m_nextActionTime)
            {
                ChangeState(new IdleBehaviourState(Behaviour, true));
            }
        }

        public override void OnStateExit()
        {
            Monster.HardLookAround(false);
            Monster.SetHostileLevel(HostileLevel.Calm);
        }
    }
    
    // ======== Suspect State ========
    
    private class SuspectBehaviourState : BehaviourState
    {
        private readonly bool m_fromCaution;
        private float m_suspectProgress;

        public SuspectBehaviourState(MonsterBehaviour behaviour, bool fromCaution = false) : base(behaviour)
        {
            m_fromCaution = fromCaution;
        }

        public override void OnStateEnter()
        {
            m_suspectProgress = m_fromCaution ? 0.5f : 0f;
            Monster.LookAt(Behaviour.PlayerPosition);
        }

        public override void OnStateUpdate()
        {
            if (Monster.WithinForwardSight(Behaviour.PlayerPosition, Behaviour.PlayerHeight))
            {
                m_suspectProgress += 1f / Behaviour.m_screamDuration * Time.deltaTime;
                Monster.LookAt(Behaviour.PlayerPosition);
            }
            else
            {
                m_suspectProgress -= 1f / Behaviour.m_easeDuration * Time.deltaTime;
            }
            
            switch (m_suspectProgress)
            {
                case <= 0f:
                    ChangeState(new IdleBehaviourState(Behaviour, true));
                    break;
                case > 0f and <= 0.1f:
                    Monster.SetHostileLevel(HostileLevel.Calm);
                    break;
                case > 0.1f and <= 0.5f:
                    Monster.SetHostileLevel(HostileLevel.Caution);
                    break;
                case > 0.5f and < 1f:
                    Monster.SetHostileLevel(HostileLevel.Alert);
                    break;
                default:
                    ChangeState(new ScreamBehaviourState(Behaviour, Behaviour.PlayerPosition));
                    break;
            }
        }

        public override void OnStateExit()
        {
            Monster.CancelLookAt();
        }
    }
    
    // ======== Scream State ========
    
    private class ScreamBehaviourState : BehaviourState
    {
        private readonly Vector3 m_targetPosition;
        private float m_nextActionTime;

        public ScreamBehaviourState(MonsterBehaviour behaviour, Vector3 targetPosition) : base(behaviour)
        {
            m_targetPosition = targetPosition;
        }

        public override void OnStateEnter()
        {
            m_nextActionTime = Time.time + Behaviour.m_screamDuration;
            Monster.Scream();
            Monster.SetHostileLevel(HostileLevel.Hostile);
        }

        public override void OnStateUpdate()
        {
            if (Time.time > m_nextActionTime)
            {
                ChangeState(new ChaseBehaviourState(Behaviour, m_targetPosition)); 
            }
        }

        public override void OnStateExit()
        {
        }
    }
    
    // ======== Chase State ========
    
    private class ChaseBehaviourState : BehaviourState
    {
        private readonly Vector3 m_initialTargetPosition;

        public ChaseBehaviourState(MonsterBehaviour behaviour, Vector3 targetPosition) : base(behaviour)
        {
            m_initialTargetPosition = targetPosition;
        }

        public override void OnStateEnter()
        {
            Monster.ChaseTo(m_initialTargetPosition);
        }

        public override void OnStateUpdate()
        {
            if (Monster.WithinPeripheralSight(Behaviour.PlayerPosition, Behaviour.PlayerHeight))
            {
                Monster.ChaseTo(Behaviour.PlayerPosition);
            }
            else if (Monster.HasArrivedToTarget())
            {
                ChangeState(new TargetLostBehaviourState(Behaviour)); 
            }
        }

        public override void OnStateExit()
        {
            Monster.CancelNavigation();
        }
    }
    
    // ======== Target Lost State ========
    
    private class TargetLostBehaviourState : BehaviourState
    {
        private float m_nextActionTime;
        private float m_halfActionTime;
        private bool m_pastHalfTime;

        public TargetLostBehaviourState(MonsterBehaviour behaviour) : base(behaviour) { }

        public override void OnStateEnter()
        {
            m_nextActionTime = Time.time + Behaviour.m_targetLostDuration;
            m_halfActionTime = Time.time + Behaviour.m_targetLostDuration / 2f;
            Monster.HardLookAround(true);
            Monster.SetHostileLevel(HostileLevel.Alert);
        }

        public override void OnStateUpdate()
        {
            if (!m_pastHalfTime && Time.time > m_halfActionTime)
            {
                Monster.SetHostileLevel(HostileLevel.Caution);
                m_pastHalfTime = true;
            }
            
            if (Monster.WithinForwardSight(Behaviour.PlayerPosition, Behaviour.PlayerHeight))
            {
                ChangeState(new ScreamBehaviourState(Behaviour, Behaviour.PlayerPosition));
            }
            else if (Time.time > m_nextActionTime)
            {
                ChangeState(new IdleBehaviourState(Behaviour, true));
            }
        }

        public override void OnStateExit()
        {
            Monster.HardLookAround(false);
            Monster.SetHostileLevel(HostileLevel.Calm);
        }
    }
}
