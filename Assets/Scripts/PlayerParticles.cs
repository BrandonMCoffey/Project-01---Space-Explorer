using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts {
    public class PlayerParticles : MonoBehaviour {
        [SerializeField] private float _particleMultiplier = 2;
        [SerializeField] private List<ParticleSystem> _movementParticles = null;
        [SerializeField] private List<ParticleSystem.MinMaxGradient> _boostColors = null;
        [SerializeField] private List<ParticleSystem> _boostParticles = null;
        [SerializeField] private List<ParticleSystem> _sizeChangeParticles = null;
        [SerializeField] private List<ParticleSystem> _laserHitParticles = null;
        [SerializeField] private List<ParticleSystem.MinMaxGradient> _laserBoostColors = null;
        [SerializeField] private List<ParticleSystem> _deathParticles = null;

        private int _currentBoostLevel;
        private int _currentLaserLevel;

        private void Start()
        {
            ChangeBoostColor(0);
            ChangeLaserColor(0);
        }

        public void UpdateMovementParticles(float amount)
        {
            if (_movementParticles == null) return;
            foreach (ParticleSystem system in _movementParticles) {
                ParticleSystem.EmissionModule emission = system.emission;
                emission.rateOverTime = amount * _particleMultiplier;
            }
        }

        public void PlayBoostParticles()
        {
            if (_boostParticles == null) return;
            foreach (ParticleSystem system in _boostParticles) {
                system.Play();
            }
        }

        public void PlaySizeChangeParticles()
        {
            if (_sizeChangeParticles == null) return;
            foreach (ParticleSystem system in _sizeChangeParticles) {
                system.Play();
            }
        }

        public void PlayLaserHitParticles(Vector3 position, bool boosted)
        {
            if (_laserHitParticles == null) return;
            foreach (ParticleSystem system in _laserHitParticles) {
                system.transform.position = position;
                system.Emit(Mathf.CeilToInt(system.emission.rateOverTime.constant));
            }
        }

        public void PlayDeathParticles()
        {
            if (_deathParticles == null) return;
            foreach (ParticleSystem system in _deathParticles) {
                system.Play();
            }
        }

        public void ChangeBoostColor(int multiplier)
        {
            if (_boostColors == null || _boostColors.Count <= _currentBoostLevel + multiplier || _currentBoostLevel + multiplier < 0) return;
            if (multiplier == 0) {
                _currentBoostLevel = 0;
            } else {
                _currentBoostLevel += multiplier;
            }
            foreach (ParticleSystem system in _movementParticles) {
                ParticleSystem.ColorOverLifetimeModule module = system.colorOverLifetime;
                module.color = _boostColors[_currentBoostLevel];
            }
        }

        public void ChangeLaserColor(int multiplier)
        {
            if (_laserBoostColors == null || _laserBoostColors.Count <= _currentLaserLevel + multiplier || _currentLaserLevel + multiplier < 0) return;
            if (multiplier == 0) {
                _currentLaserLevel = 0;
            } else {
                _currentLaserLevel += multiplier;
            }
            foreach (ParticleSystem system in _laserHitParticles) {
                ParticleSystem.MainModule module = system.main;
                module.startColor = _laserBoostColors[_currentLaserLevel];
            }
        }

        public void Reload()
        {
            foreach (ParticleSystem system in _movementParticles) {
                system.Clear();
            }
            foreach (ParticleSystem system in _deathParticles) {
                system.Clear();
            }
        }
    }
}