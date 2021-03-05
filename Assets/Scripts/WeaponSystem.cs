using System.Collections;
using UnityEngine;

namespace Assets.Scripts {
    public class WeaponSystem : MonoBehaviour {
        [SerializeField] private LineRenderer _instantLaser = null;
        [SerializeField] private Material _boostedLaser = null;
        [SerializeField] private float _laserTimer = 0.1f;
        [SerializeField] private LayerMask _objectsToHit = 0;
        [SerializeField] private LayerMask _objectsToKill = 0;
        [SerializeField] private AudioClip _clip = null;

        private Coroutine _laserCoroutine;
        private Material _regularLaser;
        private bool _isLaserBoosted;

        private AudioSource _source;


        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _regularLaser = _instantLaser.material;
            if (_boostedLaser == null) {
                Debug.Log("[WeaponsSystem] Warning: Missing Boosted Laser Sprite");
            }
        }

        private void Start()
        {
            _instantLaser.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_instantLaser == null) return;
            _instantLaser.SetPosition(0, transform.position);
        }

        public void Fire(PlayerParticles particles)
        {
            _source.clip = _clip;
            _source.Play();
            Physics.Raycast(transform.position, transform.forward, out var hit, Mathf.Infinity, _objectsToHit);
            if (_isLaserBoosted) {
                while (true) {
                    Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, _objectsToHit);
                    if (hit.transform == null) return;
                    if (_objectsToKill == (_objectsToKill | (1 << hit.transform.gameObject.layer))) {
                        EnemyAI enemyAI = hit.transform.GetComponent<EnemyAI>();
                        if (enemyAI != null) {
                            enemyAI.Kill();
                        } else {
                            hit.transform.gameObject.SetActive(false);
                        }
                        particles.PlayLaserHitParticles(hit.point, true);
                    } else {
                        break;
                    }
                }
            } else {
                if (hit.transform == null) return;
                if (_objectsToKill == (_objectsToKill | (1 << hit.transform.gameObject.layer))) {
                    EnemyAI enemyAI = hit.transform.GetComponent<EnemyAI>();
                    if (enemyAI != null) {
                        enemyAI.Kill();
                    } else {
                        hit.transform.gameObject.SetActive(false);
                    }
                }
            }

            if (_instantLaser == null) {
                Debug.Log("[WeaponSystem] Warning: No Instant Laser Attached to " + gameObject.name);
            } else {
                particles.PlayLaserHitParticles(hit.point, _isLaserBoosted);
                if (_laserCoroutine != null) {
                    StopCoroutine(_laserCoroutine);
                }
                _laserCoroutine = StartCoroutine(FireLaser(hit.point));
            }
        }

        private IEnumerator FireLaser(Vector3 pos)
        {
            _instantLaser.gameObject.SetActive(true);
            _instantLaser.SetPosition(1, pos);
            for (float t = 0; t < _laserTimer; t += Time.fixedDeltaTime) {
                yield return null;
            }
            _instantLaser.gameObject.SetActive(false);
        }

        public void BoostLaser(bool action)
        {
            _isLaserBoosted = action;
            if (_boostedLaser == null) return;
            _instantLaser.material = action ? _boostedLaser : _regularLaser;
        }
    }
}