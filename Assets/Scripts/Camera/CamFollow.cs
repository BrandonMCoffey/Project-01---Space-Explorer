using UnityEngine;

namespace Assets.Scripts.Camera {
public class CamFollow : MonoBehaviour {
    [SerializeField] private Transform _objectToFollow = null;
    [SerializeField] private bool _rotateWithObject = false;
    [SerializeField] private float _predictiveFollow = 0;
    [SerializeField] private bool _canZoom = true;
    [SerializeField] private float _minZoom = 0.5f;
    [SerializeField] private float _maxZoom = 2.5f;
    [SerializeField] private Transform _transformOverride = null;

    [HideInInspector] public Vector3 _objectOffset;
    private float _currentZoom = 1f;

    private void OnEnable()
    {
        if (_objectToFollow != null) {
            // Create an offset between this position and the other object's position
            _objectOffset = transform.position - _objectToFollow.position;
        } else {
            // Warn the user that there is no attached object to follow
            Debug.Log("CamFollow: No Object to Follow attached to \"" + gameObject.name + "\"");
        }
    }

    private void FixedUpdate()
    {
        if (_objectToFollow == null) return;
        // Apply the offset every frame, to reposition this object
        Rigidbody rb = _objectToFollow.GetComponent<Rigidbody>();
        if (rb != null) {
            transform.position = _objectToFollow.position + (rb.velocity + _objectToFollow.forward * rb.velocity.magnitude / 5) * _predictiveFollow + _objectOffset;
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