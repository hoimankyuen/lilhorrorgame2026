using System;
using System.Collections.Generic;
using MoonlightTools.GizmoTools;
using UnityEngine;

public class Provoker : MonoBehaviour
{
    private static readonly List<Provoker> s_provokers = new();
    private static readonly HashSet<Lure> s_broadcastingProvokers = new();
    public static Action BroadcastingProvokerChanged; 
    
    [SerializeField] private float m_maxRange;
    
    private bool m_isBroadcasting;
    
    // ======== Unity Events ========

    private void Awake()
    {
        s_provokers.Add(this);
    }

    private void OnDestroy()
    {
        s_provokers.Remove(this);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        GizmoUtils.DrawCircle(transform.position, m_maxRange);
    }
    
    // ======== Effects ========

    public void Broadcast(bool active)
    {
        m_isBroadcasting = active;
        BroadcastingProvokerChanged?.Invoke();
    }
}