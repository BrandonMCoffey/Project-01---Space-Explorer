using UnityEngine;

namespace Assets.Scripts.Camera {
    public class CamFollow : MonoBehaviour {
        [Header("Required References")]
        [SerializeField] private Transform _objectToFollow = null;

        [Header("Settings")]
        [SerializeField] private bool _rotateWithObject = false;
        [SerializeField] private float _predictiveFollow = 0;
        [SerializeField] private bool _canZoom = true;
        [SerializeField] private float _minZoom = 0.5f;
        [SerializeField] private float _maxZoom = 2.5f;

        [Header("Overrides")]
        [SerializeField] private Transform _transformOverride = null;
        public float _cameraShakeOverride = 1;

        [HideInInspector] public Vector3 _objectOffset;
        private Rigidbody _objectToFollowRigidbody = null;
        private float _currentZoom = 1f;

        private void OnEnable()
        {
            if (_objectToFollow != null) {
                // Create an offset between this position and the other object's position
                _objectOffset = transform.position - _objectToFollow.position;
                _objectToFollowRigidbody = _objectToFollow.GetComponent<Rigidbody>();
            } else {
                // Warn the user that there is no attached object to follow
                Debug.Log("CamFollow: No Object to Follow attached to \"" + gameObject.name + "\"");
            }
        }

        private void FixedUpdate()
        {
            if (_objectToFollow == null) return;
            // Apply the offset every frame, to reposition this object
            if (_objectToFollowRigidbody != null) {
                Vector3 predictiveFollow = _objectToFollow.forward * Input.GetAxis("Vertical") * _objectToFollowRigidbody.velocity.magnitude * _predictiveFollow;
                transform.position = _objectToFollow.position + _objectOffset + predictiveFollow;
            } else {
                transform.position = _objectToFollow.position + _objectOffset;
            }

            if (_rotateWithObject) {
                transform.rotation = _objectToFollow.rotation;
            }
        }

        public Transform GetTransform()
        {
            return _transformOverride != null ? _transformOverride : transform;
        }

        public Vector3 GetRespawnPosition()
        {
            return _transformOverride != null ? _objectOffset + _transformOverride.localPosition : _objectOffset;
        }

        public void Zoom(float amount)
        {
            if (!_canZoom) return;
            _currentZoom += amount;
            _currentZoom = Mathf.Clamp(_currentZoom, _minZoom, _maxZoom);

            // FIXME
            transform.localScale = new Vector3(_currentZoom, _currentZoom, _currentZoom);
        }
    }
}