using System;
using System.Collections.Generic;
using MoonlightTools.GizmoTools;
using UnityEngine;

public class Lure : MonoBehaviour
{
    private static readonly List<Lure> s_lures = new();
    private static readonly HashSet<Lure> s_broadcastingLures = new();
    public static event Action BroadcastingLuresChanged;

    [Header("Lure Components")]   
    [SerializeField] protected Animator m_animator;
    [SerializeField] protected SoundParticleEffect m_soundParticleEffect;
    
    [Header("Lure Settings")]
    [SerializeField] private float m_maxRange;
    [SerializeField] private string m_activeAnimationParam;
    
    public bool IsBroadcasting { get; private set; }
    
    // ======== Unity Events ========

    protected virtual void Awake()
    {
        s_lures.Add(this);
    }

    protected virtual void OnDestroy()
    {
        s_lures.Remove(this);
        s_broadcastingLures.Remove(this);
    }
    
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        GizmoUtils.DrawCircle(transform.position, m_maxRange);
    }
    
    // ======== Query ========

    public static Lure GetNearestBroadcastingLure(Vector3 position)
    {
        Lure nearestLure = null;
        float minDistance = float.MaxValue;
        foreach (Lure lure in s_broadcastingLures)
        {
            float distance = Vector3.Distance(position, lure.transform.position);
            if (distance > lure.m_maxRange || distance > minDistance)
                continue;
            
            nearestLure = lure;
            minDistance = distance;
        }
        return nearestLure;
    }
    
    // ======== Effects ========

    public virtual void ChangeActiveState(bool active)
    {
        if (active)
        {
            s_broadcastingLures.Add(this);
        }
        else
        {
            s_broadcastingLures.Remove(this);
        }

        IsBroadcasting = active;
        m_soundParticleEffect.Play(active);
        m_animator.SetBool(m_activeAnimationParam, active);
        
        BroadcastingLuresChanged?.Invoke();
    }
}
