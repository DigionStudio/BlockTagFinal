using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleColor : MonoBehaviour
{
    public ParticleSystem[] ParticleSystems;
    

    public void ChangeParticleColor(Color color)
    {
        foreach (var particle in ParticleSystems)
        {
            ParticleSystem.MainModule main = particle.main;
            main.startColor = new ParticleSystem.MinMaxGradient(color);
        }
    }
}
