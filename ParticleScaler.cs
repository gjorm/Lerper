   using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
[ExecuteInEditMode()]
public class ParticleScaler : MonoBehaviour
{

    ParticleSystem m_System;
    ParticleSystem.Particle[] m_Particles;
    public float m_Size = 0.1f;
    public float m_StartSpeed = 0.1f;

    public void Start()
    {
        InitializeIfNeeded();

        // GetParticles is allocation free because we reuse the m_Particles buffer between updates
        int numParticlesAlive = m_System.GetParticles(m_Particles);

        //float currentScale = (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3.0f;
        var foo = m_System.main;
        //foo.startSpeed = m_StartSpeed;
        foo.startSize = m_Size;

        float cur;
        for (int i = 0; i < numParticlesAlive; i++)
        {
            cur = m_Particles[i].startSize;
            m_Particles[i].startSize = m_Size * cur;
        }

        m_System.SetParticles(m_Particles, numParticlesAlive);

    }

    void InitializeIfNeeded()
    {

        if (m_System == null)
            m_System = GetComponent<ParticleSystem>();

        if (m_Particles == null || m_Particles.Length < m_System.main.maxParticles)
            m_Particles = new ParticleSystem.Particle[m_System.main.maxParticles];
    }

}