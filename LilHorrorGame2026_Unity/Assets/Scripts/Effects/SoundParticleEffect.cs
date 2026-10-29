using System;
using UnityEngine;

public class SoundParticleEffect : MonoBehaviour
{
     [SerializeField] private ParticleSystem m_normalParticle;
     [SerializeField] private ParticleSystem m_enlargedParticle;
     [SerializeField] private float m_billboardOffset;

     private Camera m_camera;
     private bool m_isPlaying;
     
     // ======== Unity Events ========

     private void Awake()
     {
          m_camera = Camera.main;
          m_normalParticle.transform.localPosition = Vector3.forward * m_billboardOffset;
          m_enlargedParticle.transform.localPosition = Vector3.forward * m_billboardOffset;
     }
     
     private void Update()
     {
          Billboarding();
     }
     
     // ======== Functionality ========

     private void Billboarding()
     {
          if (m_camera == null)
               return;

          if (!m_isPlaying)
               return;
          
          transform.LookAt(m_camera.transform);
     }

     public void Play(bool play)
     {
          m_isPlaying = play;
          if (play)
          {
               m_normalParticle.Play();
               m_enlargedParticle.Play();
          }
          else
          {
               m_normalParticle.Stop();
               m_enlargedParticle.Stop();
          }
     }
}
