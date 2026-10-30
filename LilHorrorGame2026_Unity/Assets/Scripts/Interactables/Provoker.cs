using System;
using System.Collections.Generic;
using MoonlightTools.GizmoTools;
using UnityEngine;

public class Provoker : MonoBehaviour
{
    
    
    
    [Header("Interactable Components")]   
    [SerializeField] protected Animator m_animator;
    [SerializeField] protected SoundParticleEffect m_soundParticleEffect;

    [Header("Interactable Settings")]   
    [SerializeField] private float m_maxRange;
    [SerializeField] private string m_activeAnimationParam;

    [Header("Provoker Settings")] 
    [SerializeField] private float m_sanityAmount;
    
    public bool IsBroadcasting { get; private set; }

    private void Start()
    {
        ProvokerManager.Instance.RegisterProvoker(this);
    }

    // ======== Unity Events ========
    
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        GizmoUtils.DrawCircle(transform.position, m_maxRange);
    }
    
    // ======== Effects ========

    public virtual void ChangeActiveState(bool active)
    {
        IsBroadcasting = active;
        m_soundParticleEffect.Play(active);
        m_animator.SetBool(m_activeAnimationParam, active);
    }
    
    public bool GetActiveState()
    {
        return IsBroadcasting;
    }
}