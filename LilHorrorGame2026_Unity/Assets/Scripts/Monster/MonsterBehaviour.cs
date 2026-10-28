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
    }

    [Header("Shared Settings")]
    [SerializeField] private Transform m_playerBodyTransform; // to be changed
    [SerializeField] private Transform m_playerHeadTransform; // to be changed
    
    [Header("Idle State Settings")]
    [SerializeField] private RangedFloat m_idleDuration;
    [SerializeField] private RangedFloat m_idleExtensionDuration;
    [SerializeField] private float m_idleChance;
    [SerializeField] private float m_turnChance;
    [SerializeField] private float m_wanderChance;
    [SerializeField] private float m_lookChance;
    [SerializeField] private float m_screamChance;
    
    [Header("Turn State Settings")]
    [SerializeField] private float m_minTurnAngle;
    
    [Header("Wander State Settings")]
    [SerializeField] private MonsterPatrolPointSet m_wanderPatrolPointSet;
    
    [Header("Look State Settings")]
    [SerializeField] private float m_lookDuration;
    
    [Header("Scream State Settings")]
    [SerializeField] private float m_screamDuration;
    
    [Header("Suspect State Settings")]
    [SerializeField] private float m_suspectDuration;
    [SerializeField] private float m_easeDuration;

    private Monster m_monster;
    
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
        ChangeState(new IdleBehaviourState(this));
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
        }
    }

    private void ChangeState(BehaviourState nextState)
    {
        m_nextState = nextState;
        m_changingState = true;
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

        public IdleBehaviourState(MonsterBehaviour behaviour, bool isExtension = false) : base(behaviour)
        {
            m_isExtension = isExtension;
        }
        
        public override void OnStateEnter()
        {
            m_nextActionTime = Time.time + (m_isExtension ? Behaviour.m_idleDuration.Random() : Behaviour.m_idleExtensionDuration.Random());
        }

        public override void OnStateUpdate()
        {
            if (Monster.CanDetect(Behaviour.PlayerPosition, Behaviour.PlayerHeight))
            {
                Behaviour.ChangeState(new SuspectBehaviourState(Behaviour));
            }
            else if (Time.time > m_nextActionTime)
            {
                // note: require refactoring
                float[] chances =
                {
                    Behaviour.m_idleChance,
                    Behaviour.m_turnChance,
                    Behaviour.m_wanderChance,
                    Behaviour.m_lookChance,
                    Behaviour.m_screamChance
                };
                switch (GetFromWeightedChances(chances))
                {
                    case 0:
                        ChangeState(new IdleBehaviourState(Behaviour, true));
                        break;
                    case 1:
                        ChangeState(new TurnBehaviourState(Behaviour));
                        break;
                    case 2:
                        ChangeState(new WanderBehaviourState(Behaviour));
                        break;
                    case 3:
                        ChangeState(new LookBehaviourState(Behaviour));
                        break;
                    case 4:
                        ChangeState(new ScreamBehaviourState(Behaviour));
                        break;
                    default:
                        ChangeState(new IdleBehaviourState(Behaviour, true));
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
            if (Monster.CanDetect(Behaviour.PlayerPosition, Behaviour.PlayerHeight))
            {
                ChangeState(new SuspectBehaviourState(Behaviour));
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
            if (Monster.CanDetect(Behaviour.PlayerPosition, Behaviour.PlayerHeight))
            {
                ChangeState(new SuspectBehaviourState(Behaviour));
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
            Monster.LookAround(true);
        }

        public override void OnStateUpdate()
        {

            if (Monster.CanDetect(Behaviour.PlayerPosition, Behaviour.PlayerHeight))
            {
                ChangeState(new SuspectBehaviourState(Behaviour));
            }
            else if (Time.time > m_nextActionTime)
            {
                ChangeState(new IdleBehaviourState(Behaviour, true)); 
            }
        }

        public override void OnStateExit()
        {
            Monster.LookAround(false);
        }
    }
    
    // ======== Scream State ========
    
    private class ScreamBehaviourState : BehaviourState
    {
        private float m_nextActionTime;
        
        public ScreamBehaviourState(MonsterBehaviour behaviour) : base(behaviour) {}

        public override void OnStateEnter()
        {
            m_nextActionTime = Time.time + Behaviour.m_screamDuration;
            Monster.Scream();
        }

        public override void OnStateUpdate()
        {
            if (Time.time > m_nextActionTime)
            {
                ChangeState(new IdleBehaviourState(Behaviour, true)); 
            }
        }

        public override void OnStateExit()
        {
        }
    }
    
    // ======== Suspect State ========
    
    private class SuspectBehaviourState : BehaviourState
    {
        private float m_suspectProgress;
        
        public SuspectBehaviourState(MonsterBehaviour behaviour) : base(behaviour) {}

        public override void OnStateEnter()
        {
            m_suspectProgress = 0f;
            Monster.LookAt(Behaviour.PlayerPosition);
        }

        public override void OnStateUpdate()
        {
            if (Monster.CanDetect(Behaviour.PlayerPosition, Behaviour.PlayerHeight))
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
                    Monster.SetHostileLevel(Monster.HostileLevel.Calm);
                    break;
                case > 0.1f and <= 0.5f:
                    Monster.SetHostileLevel(Monster.HostileLevel.Caution);
                    break;
                case > 0.5f and < 1f:
                    Monster.SetHostileLevel(Monster.HostileLevel.Alert);
                    break;
                default:
                    ChangeState(new ChaseBehaviourState(Behaviour));
                    break;
            }
        }

        public override void OnStateExit()
        {
            Monster.CancelLookAt();
        }
    }
    
    // ======== Scream State ========
    
    private class ChaseBehaviourState : BehaviourState
    {
        public ChaseBehaviourState(MonsterBehaviour behaviour) : base(behaviour) {}

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
}
