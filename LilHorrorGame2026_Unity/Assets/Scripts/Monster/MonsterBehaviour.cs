using UnityEngine;

[RequireComponent(typeof(Monster))]
public class MonsterBehaviour : MonoBehaviour
{
    private class BehaviourState
    {
        private Monster m_monster;
        private MonsterBehaviour m_behaviour;

        public BehaviourState(MonsterBehaviour behaviour)
        {
             m_behaviour = behaviour;
             m_monster = behaviour.Monster;
        }
        
        public virtual void OnStateEnter() { }
        public virtual void OnStateUpdate() { }
        public virtual void OnStateExit() { }
    }
    
    private BehaviourState m_currentState;
    private BehaviourState m_nextState;

    public Monster Monster { get; private set; }

    
    // ======== Unity Events ========
    
    private void Awake()
    {
        Monster = GetComponent<Monster>();
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
        if (m_currentState != null)
        {
            m_currentState.OnStateUpdate();
        }
        if (m_nextState != null)
        {
            m_currentState.OnStateExit();
            m_currentState = m_nextState;
            m_nextState = null;
            m_currentState.OnStateEnter();
        }
    }

    private void ChangeState(BehaviourState nextState)
    {
        m_nextState = nextState;
    }
    
    // ======== Idle State ========

    private class IdleBehaviourState : BehaviourState
    {
        public IdleBehaviourState(MonsterBehaviour behaviour) : base(behaviour) {}
        
        
    }
}
