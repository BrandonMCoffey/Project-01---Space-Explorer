using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts {
public class CameraController : MonoBehaviour {
    [SerializeField] private Transform _objectToFollow = null;
    [Range(0f, 1f)] [SerializeField] private float _cameraSmoothing = 0.5f;
    [SerializeField] private float _minZoom = 0.5f;
    [SerializeField] private float _maxZoom = 2.5f;

    [HideInInspector] private bool _resetting = false;

    private Vector3 _objectOffset;
    private bool _rotateWithObject;
    private float _currentZoom = 1;
    private List<GameObject> _cameras;
    private int _currentCamera;

    private void Awake()
    {
        if (_objectToFollow != null) {
            // Create an offset between this position and the other object's position
            _objectOffset = transform.position - _objectToFollow.position;
        } else {
            // Warn the user that there is no attached object to follow
            Debug.Log("[CameraFollow] No Object to Follow connected to \"" + gameObject.name + "\"");
        }

        // Initiate a new list of cameras
        _cameras = new List<GameObject>();
        bool activateFirst = true;

        // Search through the children of this transform
        foreach (Transform child in transform) {
            // If the transform has a connected camera, add it to the list
            if (child.GetComponent<CameraOptions>()) {
                _cameras.Add(child.gameObject);
                child.gameObject.SetActive(activateFirst);
                activateFirst = false;
            }
        }

        // Warn the user if no cameras were found
        if (_cameras.Count < 1) {
            Debug.Log("[CameraSwitch] No connected cameras");
        }
    }

    private void FixedUpdate()
    {
        if (_objectToFollow == null) return;
        // Smooth out the position
        Vector3 desiredPosition = _objectToFollow.position + _objectOffset;
        Rigidbody rb = _objectToFollow.GetComponent<Rigidbody>();
        if (rb != null) {
            desiredPosition += 1.1f * rb.velocity;
        }
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, _resetting ? Vector3.zero : desiredPosition, _cameraSmoothing);
        // Apply the offset every frame, to reposition this object
        transform.position = smoothedPosition;
        // Apply the rotation every frame if applicable
        if (_rotateWithObject) {
            transform.rotation = _objectToFollow.rotation;
        }
    }

    public void Zoom(float amount)
    {
        // Apply the zoom and clamp it to the min and max values
        _currentZoom += amount;
        _currentZoom = Mathf.Clamp(_currentZoom, _minZoom, _maxZoom);

        // Apply the zoom amount to the scale of this object
        transform.localScale = new Vector3(_currentZoom, _currentZoom, _currentZoom);
    }

    public void RotateThroughCameras(bool reverse = false)
    {
        if (_cameras == null || _cameras.Count < 1) return;
        // Disable the active camera
        _cameras[_currentCamera].SetActive(false);
        // Iterate the current camera
        _currentCamera += reverse ? -1 : 1;
        // If the end of the list is reached, reset to start / end of list
        if (_currentCamera >= _cameras.Count || _currentCamera < 0) {
            _currentCamera = reverse ? _cameras.Count - 1 : 0;
        }

        // Activate the new active camera
        _cameras[_currentCamera].SetActive(true);
        _rotateWithObject = _cameras[_currentCamera].GetComponent<CameraOptions>()._rotateWithObject;
        if (!_rotateWithObject) {
            transform.rotation = Quaternion.identity;
        }
    }

    public IEnumerator ResetCamera(int timer)
    {
        yield return new WaitForSeconds(1);
        _resetting = true;

        yield return new WaitForSeconds(timer - 1);
        _resetting = false;
    }
}
}