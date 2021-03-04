using System.Collections;
using UnityEngine;

namespace Assets.Scripts {
    [RequireComponent(typeof(Rigidbody))]
    public class HomingProjectile : MonoBehaviour {
        public Transform _objectToFollow;

        [SerializeField] private float _turnSpeed = 5;
        [SerializeField] private int _lifeTime = 10;
        [SerializeField] private ParticleSystem _explosionParticles = null;

        private Rigidbody _rigidbody;
        private Coroutine _deathCoroutine;
        private bool _isAlive = true;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _deathCoroutine = StartCoroutine(Death());
        }

        private void Update()
        {
            if (!_isAlive) return;
            if (_objectToFollow != null) {
                Vector3 cross = Vector3.Cross(_objectToFollow.position, transform.forward);
                _rigidbody.angularVelocity = -cross * _turnSpeed;
            } else {
                _rigidbody.velocity = Vector3.forward;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!_isAlive) return;
            if (other.gameObject.layer == LayerMask.NameToLayer("Player")) return;
            if (other.transform == _objectToFollow) {
                Destroy(other.gameObject);
            }
            Kill();
        }

        private IEnumerator Death()
        {
            yield return new WaitForSeconds(_lifeTime);
            Kill();
        }

        private void Kill()
        {
            _isAlive = false;
            StopCoroutine(_deathCoroutine);
            Transform art = transform.Find("Art");
            art.gameObject.SetActive(false);
            if (_explosionParticles != null) {
                StartCoroutine(Explosion());
            } else {
                Destroy(gameObject);
            }
        }

        private IEnumerator Explosion()
        {
            _explosionParticles.Play();
            yield return new WaitForSeconds(Mathf.Ceil(_explosionParticles.main.duration));
            Destroy(gameObject);
        }
    }
}