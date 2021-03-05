using UnityEngine;

namespace Assets.Scripts {
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class EnemyAI : MonoBehaviour {
        [SerializeField] private GameObject _art = null;
        [SerializeField] private float _moveSpeed = 10f;
        [SerializeField] private float _turnDrag = 5f;
        [SerializeField] private float _lookRadius = 10f;
        [SerializeField] private float _lostRadius = 15f;
        [SerializeField] private float _stoppingDistance = 4f;
        [SerializeField] private float _bufferDistance = 1f;
        [SerializeField] private bool _canFireLaser = false;

        private Rigidbody _rigidbody;
        private Collider[] _colliders;
        private Vector3 _startingPosition;
        private Quaternion _startingRotation;
        private bool _isAlive = true;

        private Transform _target;
        private AIData _data;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _colliders = GetComponents<Collider>();
            _startingPosition = transform.position;
            _startingRotation = transform.rotation;
            if (_art == null) {
                Debug.Log("[EnemyAI] Warning: No Art connected");
            }
        }

        private void Start()
        {
            _target = GameController._instance._playerShip.transform;
        }

        private void Update()
        {
            if (!_isAlive) return;
            float distanceToPlayer = Vector3.Distance(transform.position, _target.position);
            if (!_data._canSeeTarget) {
                _data._canSeeTarget = distanceToPlayer <= _lookRadius;
            } else {
                _data._canSeeTarget = distanceToPlayer <= _lostRadius;
            }
            _data._firingDistance = distanceToPlayer <= _stoppingDistance + _bufferDistance;
            _data._tooCloseToTarget = distanceToPlayer < _stoppingDistance - _bufferDistance;
        }

        private void FixedUpdate()
        {
            if (!_isAlive) return;
            Turn();
            Move();
        }

        private void Move()
        {
            if (!_data._canSeeTarget) return;
            if (!_data._firingDistance) {
                _rigidbody.AddForce(transform.forward * _moveSpeed);
            } else if (_data._tooCloseToTarget) {
                _rigidbody.AddForce(-transform.forward * _moveSpeed);
            }
        }

        private void Turn()
        {
            if (!_data._canSeeTarget) return;
            Quaternion current = transform.rotation;
            transform.LookAt(_target);
            Quaternion goal = transform.rotation;
            Quaternion newRotation = Quaternion.Lerp(current, goal, 1 / _turnDrag);
            transform.rotation = newRotation;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, _lostRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _lookRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _stoppingDistance + _bufferDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _stoppingDistance - _bufferDistance);
        }

        public void Kill()
        {
            _isAlive = false;
            foreach (var col in _colliders) {
                col.enabled = false;
            }
            _rigidbody.velocity = Vector3.zero;
            if (_art != null) {
                _art.SetActive(false);
            }
        }

        public void Reload()
        {
            _isAlive = true;
            foreach (var col in _colliders) {
                col.enabled = true;
            }
            transform.position = _startingPosition;
            transform.rotation = _startingRotation;
            _rigidbody.velocity = Vector3.zero;
            if (_art != null) {
                _art.SetActive(true);
            }
        }
    }

    internal struct AIData {
        public bool _canSeeTarget;
        public bool _lostTarget;
        public bool _firingDistance;
        public bool _tooCloseToTarget;
    }
}