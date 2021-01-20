using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts {
public class PlayerParticles : MonoBehaviour {
    [SerializeField] private float _particleMultiplier = 2;
    [SerializeField] private List<ParticleSystem> _movementParticles = null;
    [SerializeField] private List<ParticleSystem> _deathParticles = null;

    public void UpdateMovementParticles(float amount)
    {
        if (_movementParticles == null) return;
        foreach (ParticleSystem particleSystem in _movementParticles) {
            ParticleSystem.EmissionModule emission = particleSystem.emission;
            emission.rateOverTime = amount * _particleMultiplier;
        }
    }

    public void PlayDeathParticles()
    {
        if (_deathParticles == null) return;
        foreach (ParticleSystem particleSystem in _deathParticles) {
            particleSystem.Play();
        }
    }
}
}