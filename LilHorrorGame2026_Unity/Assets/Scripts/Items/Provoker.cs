using System;
using System.Collections.Generic;
using MoonlightTools.GizmoTools;
using UnityEngine;

public class Provoker : MonoBehaviour
{
    private static readonly List<Provoker> s_provokers = new();
    private static readonly HashSet<Provoker> s_broadcastingProvokers = new();
    public static event Action<float> BroadcastingProvokerAdded;
    public static event Action<float> BroadcastingProvokerRemoved;
    public static event Action BroadcastingProvokerChanged;
    
    [Header("Interactable Components")]   
    [SerializeField] protected Animator m_animator;
    [SerializeField] protected SoundParticleEffect m_soundParticleEffect;

    [Header("Interactable Settings")]   
    [SerializeField] private float m_maxRange;
    [SerializeField] private string m_activeAnimationParam;

    [Header("Provoker Settings")] 
    [SerializeField] private float m_sanityAmount;
    
    public bool IsBroadcasting { get; private set; }
    
    // ======== Unity Events ========

    protected virtual void Awake()
    {
        s_provokers.Add(this);
    }

    protected virtual void OnDestroy()
    {
        s_provokers.Remove(this);
    }
    
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        GizmoUtils.DrawCircle(transform.position, m_maxRange);
    }
    
    // ======== Effects ========

    public virtual void Broadcast(bool active)
    {
        if (active)
        {
            s_broadcastingProvokers.Add(this);
            BroadcastingProvokerAdded?.Invoke(m_sanityAmount);
        }
        else
        {
            s_broadcastingProvokers.Remove(this);
            BroadcastingProvokerRemoved?.Invoke(m_sanityAmount);
        }
        
        IsBroadcasting = active;
        m_soundParticleEffect.Play(active);
        m_animator.SetBool(m_activeAnimationParam, active);
        
        BroadcastingProvokerChanged?.Invoke();
    }
}