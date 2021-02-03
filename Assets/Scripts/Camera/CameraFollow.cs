using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Camera {
public class CameraFollow : MonoBehaviour {
    [SerializeField] private Transform _objectToFollow = null;
    [SerializeField] private bool _rotateWithObject = false;
    [SerializeField] private float _predictiveFollow = 0.1f;
    [SerializeField] private float _cameraSmoothing = 0.5f;
    [SerializeField] private bool _canZoom = true;
    [SerializeField] private float _minZoom = 0.5f;
    [SerializeField] private float _maxZoom = 2.5f;

    private Vector3 _objectOffset;
    private Vector3 _desiredPosition;
    private Vector3 _smoothedPosition;
    private float _currentZoom = 1f;
    private bool _resetting;

    private void OnEnable()
    {
        if (_objectToFollow != null) {
            // Create an offset between this position and the other object's position
            _objectOffset = transform.position - _objectToFollow.position;
        } else {
            // Warn the user that there is no attached object to follow
            Debug.Log("CameraFollow: No Object to Follow attached to \"" + gameObject.name + "\"");
        }
    }

    private void FixedUpdate()
    {
        if (_objectToFollow == null) return;
        // Apply the offset every frame, to reposition this object
        _desiredPosition = _objectToFollow.position + _objectOffset;

        Rigidbody rb = _objectToFollow.GetComponent<Rigidbody>();
        if (rb != null) {
            _desiredPosition += _predictiveFollow * rb.velocity;
        }

        _smoothedPosition = Vector3.Lerp(transform.position, _resetting ? Vector3.zero : _desiredPosition, _cameraSmoothing);
    }

    // LateUpdate happens after the Update (Camera should always move last)
    private void LateUpdate()
    {
        transform.position = _smoothedPosition;

        if (_rotateWithObject) {
            transform.rotation = _objectToFollow.rotation;
        }
    }

    public void Zoom(float amount)
    {
        if (!_canZoom) return;
        _currentZoom += amount;
        _currentZoom = Mathf.Clamp(_currentZoom, _minZoom, _maxZoom);

        transform.localScale = new Vector3(_currentZoom, _currentZoom, _currentZoom);
    }

    public IEnumerator ResetCamera(int timer)
    {
        yield return new WaitForSeconds(1);
        _resetting = true;

        yield return new WaitForSeconds(timer - 1);
        _resetting = false;
    }

    public void EnableCamera(bool activeState = true)
    {
        foreach (Transform child in transform) {
            child.gameObject.SetActive(activeState);
        }
    }
}
}