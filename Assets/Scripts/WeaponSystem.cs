using System.Collections;
using UnityEngine;

namespace Assets.Scripts {
    public class WeaponSystem : MonoBehaviour {
        [SerializeField] private LineRenderer _instantLaser = null;
        [SerializeField] private float _laserTimer = 0.1f;
        [SerializeField] private LayerMask _objectsToHit = 0;

        private Coroutine _laserCoroutine;

        private void Start()
        {
            _instantLaser.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_instantLaser == null) return;
            _instantLaser.SetPosition(0, transform.position);
        }

        public Vector3 Fire()
        {
            Physics.Raycast(transform.position, transform.forward, out var hit, Mathf.Infinity);
            if (_objectsToHit == (_objectsToHit | (1 << hit.transform.gameObject.layer))) {
                EnemyAI enemyAI = hit.transform.GetComponent<EnemyAI>();
                if (enemyAI != null) {
                    enemyAI.Kill();
                } else {
                    hit.transform.gameObject.SetActive(false);
                }
            }

            if (_instantLaser == null) {
                Debug.Log("[WeaponSystem] Warning: No Instant Laser Attached to " + gameObject.name);
            } else {
                if (_laserCoroutine != null) {
                    StopCoroutine(_laserCoroutine);
                }
                _laserCoroutine = StartCoroutine(FireLaser(hit.point));
            }
            return hit.point;
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
    }
}